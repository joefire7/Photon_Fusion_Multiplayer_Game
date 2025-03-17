using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SimpleCarController : NetworkBehaviour
{
    [SerializeField]
    private Wheel[] wheels = new Wheel[4]; 
    
    [SerializeField]
    private TractionMode tractionMode = TractionMode.FrontWheelDrive;
    
    [SerializeField]
    private float maxAcceleration = 30f;
    
    [SerializeField]
    private float maxNormalSpeed = 30;
    
    [SerializeField]
    private float minNormalSpeed = -15;
    
    [SerializeField]
    private float maxSpeedWithRocket = 50;
    
    [SerializeField]
    private float breakAcceleration = 20f;
    
    [SerializeField, Range(0f, 45f)]
    private float maxSteerAngle = 30f;
    
    [SerializeField]
    private float steerSensitivity = 1f;
    
    [SerializeField]
    private float jumpForce = 6000f;
    
    [SerializeField]
    private float rocketForcePerSec = 500;
    
    [SerializeField]
    private float flyRotationTorquePerSecond = 2500f;
    
    [SerializeField]
    private Vector3 centerOfMass = new Vector3(0f, 0.2f, 0.0f);

    [SerializeField, Unity.Collections.ReadOnly]
    private float speedPerSecond;
    
    [SerializeField, Unity.Collections.ReadOnly]
    private float speedPerHour;
    
    private float _horizontalInput;
    private float _verticalInput;
    private bool _isHandBrake;
    private bool _isRocketing;
    private float _steerAngle;
    private bool _canMove = true;
    private int _maxJumpRight = 2;
    private int _jumpRight = 2;
    private bool _isJumpPressed;
    private float _canJumpTime;
    private Rigidbody _rigidbody;
    private Transform _carBodyTransform;


    [SerializeField]
    private bool _isShooting;

    [SerializeField]
    private bool _isJump;
    
    [Networked] CarInputData CarInputData { get; set; }
    
    public float SpeedPerHour => speedPerHour;

    [Header("===== Paradise Splash Scripts Modify =====")]
    [SerializeField]
    private float raycastDistance = 5.0f;

    public Transform predictiedObjectTransform;
    // RAYCAST GROUND
    public bool raycastIsGrounded;
    public bool activateVFXPerGround;

    public Transform bulletSpawnPos;
    [Header("Network Object")]
    public NetworkObject playerNetworkObject;

    // Pull For to the center of the map
    public float movementSpeedCenterMap;
    [SerializeField] private bool _forceToCenter;
    public float lerpDuration = 3.0f;  // Duration for lerping
    public float lerpTimer = 0.0f; // Timer for lerping
    public float lerpProgress;
    public Transform playerTransform;
    public float moveSpeed;
    private Coroutine coroutine;

    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _carBodyTransform = _rigidbody.transform;
    }
    
    private void Start()
    {
        //var massCenter = _rigidbody.centerOfMass;
        _rigidbody.centerOfMass = new Vector3(0, centerOfMass.y, centerOfMass.z);

        Managers.Instance.GenericJumpEvent += JumpAction;
    }
    
    private void Update()
    {
        AnimateWheelMeshes();


        raycastIsGrounded = IsGrounded();
        // Draw a debug ray in the Scene view to visualize the ground checking ray
        Debug.DrawRay(predictiedObjectTransform.position, -predictiedObjectTransform.up * raycastDistance, raycastIsGrounded ? Color.green : Color.red);

        

    }
    
    public void DisableMovementAndRocket()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0;
        }
        
        _isRocketing = false;
        _verticalInput = 0;
        _canMove = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    
    public void EnableMovementAndRocket()
    {
        Debug.Log("EnableMovementAndRocket");
        _canMove = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
        _horizontalInput = CarInputData.Direction.x;
        _verticalInput = CarInputData.Direction.z;
        _isHandBrake = CarInputData.IsBraking;
        _isRocketing = CarInputData.IsRocketing;
        _isShooting = CarInputData.IsShooting;
        _isJump = CarInputData.IsJumping;
        _forceToCenter = CarInputData.IsForceToCenter;
        
        CalculateSpeed();
        Steer();
        HandBrake();

        if (_canMove && !_forceToCenter)
        {
            Move();
            Rocket();
        }

        if (_isShooting)
        {
            Managers.Instance.networkSpawnerController.SpawnBullet(bulletSpawnPos, playerNetworkObject);
        }

        if (_isJump)
        {
            JumpAction();
           
        }

        if (_forceToCenter)
        {
            Debug.Log("FORCE TO CENTER");
            //Vector3 centerMap = new Vector3(0, 0, 0);
            //MoveTowardCenter(centerMap, 20.0f);

            //transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, 50.0f * Runner.DeltaTime);

            //Vector3 distance = transform.position - Managers.Instance.ultimateAbilityManager.testCenterPos.position;

            //MoveTowardCenter(distance.normalized, 20.0f * Runner.DeltaTime);

            //transform.position = distance * Runner.DeltaTime;




            //Vector3 centerPosition = Managers.Instance.ultimateAbilityManager.testCenterPos.position; //new Vector3(0, 0, 0); // Replace this with your desired center position
            //float movementSpeed = 20.0f; // Adjust the speed as needed

            //Vector3 directionToCenter = centerPosition - transform.position;
            //float distanceToCenter = directionToCenter.magnitude;

            //if (distanceToCenter < 0.1f) // Check if the distance is significant
            //{
            //    Vector3 moveDirection = directionToCenter.normalized;
            //    float movementStep = movementSpeed * Runner.DeltaTime;

            //    if (movementStep > distanceToCenter)
            //    {
            //        transform.position = centerPosition; // Snap to center if close enough
            //    }
            //    else
            //    {
            //        transform.position += moveDirection * movementStep; // Move towards the center
            //    }
            //}



            //lerpDuration = 3.0f; // Duration for lerping
            //lerpTimer = 0.0f; // Timer for lerping
            //lerpTimer += Runner.DeltaTime;
            //lerpProgress = lerpTimer / lerpDuration;

            //// Lerp the position towards the target position
            //transform.position = Vector3.Lerp(transform.position, Managers.Instance.ultimateAbilityManager.testCenterPos.localPosition, lerpProgress);

            //if (lerpProgress >= 10.0f)
            //{
            //    // Lerp completed
            //    //isLerping = false;
            //    lerpTimer = 0.0f;
            //}



            //// Increase lerpProgress based on time
            //lerpProgress += Runner.DeltaTime / lerpDuration;

            //// Clamp the lerp progress to 1.0f to ensure it does not exceed 1
            //lerpProgress = Mathf.Clamp01(lerpProgress);

            //// Lerp the position towards the target position
            //playerTransform.transform.position = Vector3.Lerp(transform.position, Managers.Instance.ultimateAbilityManager.testCenterPos.position, lerpProgress);

            //if (lerpProgress >= 1.0f)
            //{
            //    // Lerp completed
            //    Debug.Log("Lerping completed");
            //    // Stop the lerping or handle completion
            //    // isLerping = false;
            //}


            // Calculate the direction from the current position to the target position
            Vector3 direction = (Managers.Instance.ultimateAbilityManager.testCenterPos.position - playerTransform.position).normalized;

            // Move the object towards the target position by a fraction of the direction multiplied by speed
            playerTransform.position += direction * moveSpeed * Runner.DeltaTime;

            // Check the distance between the current position and the target position
            float distance = Vector3.Distance(transform.position, Managers.Instance.ultimateAbilityManager.testCenterPos.position);

            // If the distance is very small (adjust the threshold as needed), consider the object at the target position
            if (distance < 0.01f)
            {
                //transform.position = Managers.Instance.ultimateAbilityManager.testCenterPos.localPosition;
                Debug.Log("Reached target position");
                // Consider stopping the movement or performing other actions when the target is reached
            }


            //Vector3 direction = (Managers.Instance.ultimateAbilityManager.testCenterPos.position - transform.position).normalized;
            //transform.position += direction * moveSpeed * Runner.DeltaTime;

            //float distance = Vector3.Distance(transform.position, Managers.Instance.ultimateAbilityManager.testCenterPos.position);

            //if (distance < 0.01f)
            //{
            //    transform.position = Managers.Instance.ultimateAbilityManager.testCenterPos.position;
            //    //moveToCenter = false;
            //    Managers.Instance.ultimateAbilityManager.isForceToCenter = false;
            //    Debug.Log("Reached target position");
            //    // Consider stopping the movement or performing other actions when the target is reached
            //}

            /*transform.position*/

            //playerTransform.position = Managers.Instance.ultimateAbilityManager.testCenterPos.position;

            //if(Managers.Instance.ultimateAbilityManager.isForceToCenter == true)
            //{
            //   Managers.Instance.ultimateAbilityManager.isForceToCenter = false;
            //}

        }


        // Jump
        //if (Input.GetMouseButtonUp(0))
        //{
        //    Debug.LogWarning("JUMP PRESS");

        //    if(raycastIsGrounded)
        //    {
        //        JumpAction();
        //    }
        //}



    }

    
    
    private void CalculateSpeed()
    {
        //speed at forward direction
        speedPerSecond = Vector3.Dot(_rigidbody.velocity, _carBodyTransform.forward);
        speedPerHour = speedPerSecond * 3.6f;
    }

    private void FixRotationStuck()
    {
        float yRot = playerTransform.transform.up.magnitude;
        Quaternion quaternion = Quaternion.Euler(
            0f,
            yRot,
            0f
            );

        playerTransform.rotation = quaternion;
    }
    
    private void Move()
    {
       
        if (speedPerSecond > maxNormalSpeed || speedPerSecond < minNormalSpeed)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.motorTorque = 0;
            }
            return;
        }

        foreach (var wheel in wheels)
        {
            switch (tractionMode)
            {
                case TractionMode.FrontWheelDrive:
                    if (wheel.IsFrontWheel)
                    {
                        wheel.wheelCollider.motorTorque = _verticalInput * maxAcceleration * Runner.DeltaTime * Mathf.Lerp(1.25f, 0.75f, speedPerHour / maxNormalSpeed);
                    }
                    break;
                case TractionMode.RearWheelDrive:
                    if (wheel.IsBackWheel)
                    {
                        wheel.wheelCollider.motorTorque = _verticalInput * maxAcceleration * Runner.DeltaTime * Mathf.Lerp(1.25f, 0.75f, speedPerHour / maxNormalSpeed);
                    }
                    break;
                case TractionMode.AllWheelDrive:
                    wheel.wheelCollider.motorTorque = _verticalInput * maxAcceleration * Runner.DeltaTime * Mathf.Lerp(1.25f, 0.75f, speedPerHour / maxNormalSpeed);
                    break;
            }
        }
    }
    
    private void Fly()
    {
        if (CheckIfWheelsAreGrounded())
        {
            if (Time.time > _canJumpTime)
            {
                _jumpRight = _maxJumpRight;
            }
        }//Car can fly
        else 
        {
            //Fly with torque
            _rigidbody.AddRelativeTorque(Vector3.right * flyRotationTorquePerSecond * Runner.DeltaTime * _verticalInput + 
                                         Vector3.up * flyRotationTorquePerSecond * Runner.DeltaTime * _horizontalInput, ForceMode.Impulse);
        }
    }    
    
    private void CheckJump()
    {
        //check if there is a jump input
        if (CarInputData.IsJumping)
        {
            if (_isJumpPressed)
            {
                return;
            }else if (_jumpRight > 0 && Time.time > _canJumpTime)
            {
                JumpAction();
            }
        }else
        {
            _isJumpPressed = false;
        }
        
        //check if there is a jump right
    }
    
    private void JumpAction()
    {
        //if (HasStateAuthority)
        //{
        
            if (raycastIsGrounded)
            {
                Debug.LogWarning("Jump Action!");
                _canJumpTime = Time.time + 0.15f;
                _isJumpPressed = true;
                _jumpRight--;

                var forceMode = ForceMode.VelocityChange;
                _rigidbody.AddForce(_carBodyTransform.up * jumpForce * 2.0f, forceMode);
            //if there is a horizontal input

            //if (_horizontalInput is < -0.1f or > 0.1f)
            //{
            //    //if there are horizontal and vertical inputs
            //    if (_verticalInput is < -0.1f or > 0.1f)
            //    {
            //        _rigidbody.AddForce(((_carBodyTransform.right * _horizontalInput + _carBodyTransform.forward * _verticalInput) * 2f + _carBodyTransform.up).normalized * jumpForce, forceMode);
            //        ///_rigidbody.AddTorque(_carBodyTransform.forward * -_horizontalInput * jumpForce * 0.5f + _carBodyTransform.right * _verticalInput * jumpForce * 0.25f, forceMode);
            //    }
            //    else
            //    {
            //        //Just horizontal input
            //        _rigidbody.AddForce((_carBodyTransform.right * _horizontalInput * 2.0f + _carBodyTransform.up).normalized * jumpForce, forceMode);
            //        ///_rigidbody.AddTorque(_carBodyTransform.forward * -_horizontalInput * jumpForce * 0.5f, forceMode);
            //    }
            //}
            //else
            //{
            //    //Just vertical input
            //    if (_verticalInput is < -0.1f or > 0.1f)
            //    {
            //        _rigidbody.AddForce((_carBodyTransform.forward * _verticalInput + _carBodyTransform.up * 2.0f).normalized * jumpForce, forceMode);
            //        ///_rigidbody.AddTorque(_carBodyTransform.right * jumpForce * _verticalInput * 0.35f, forceMode);
            //    }
            //    else
            //    {
            //        //Just jump
            //        _rigidbody.AddForce(_carBodyTransform.up * jumpForce * 2.0f, forceMode);
            //    }
            //}
        }
       // }
       
       
    }

    private bool CheckIfWheelsAreGrounded()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.IsGrounded)
            {
                return true;
            }
        }

        return false;
    }

    private void Steer()
    {
        _steerAngle = _horizontalInput * maxSteerAngle * steerSensitivity;
        foreach (var wheel in wheels)
        {
            if (wheel.IsFrontWheel)
            {
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, Runner.DeltaTime * 30f);
            }
        }
    }
    
    private void AnimateWheelMeshes()
    {
        foreach (var wheel in wheels)
        {
            wheel.SynchronizeWheels();
        }
    }

    private void HandBrake()
    {
        if (_isHandBrake)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = breakAcceleration * Runner.DeltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0f;
            }
        }
    }
    
    private void Rocket()
    {
        if (!_isRocketing) return;
        if (speedPerSecond > maxSpeedWithRocket) return;
        _rigidbody.AddForce(_carBodyTransform.forward * rocketForcePerSec * Runner.DeltaTime, ForceMode.Acceleration);

    }

    public void SetInputData(CarInputData data)
    {
        CarInputData = data;
    }


    //  is Grounded for Paradise Splash
    public bool IsGrounded()
    {
        bool grounded = false;
       
        if (Physics.Raycast(predictiedObjectTransform.position, -predictiedObjectTransform.up, out RaycastHit hit, raycastDistance))
        {
            //Check if the collider hit is tagged as "Ground"(modify as needed)
            if (hit.collider.CompareTag("Ground"))
            {
                Debug.LogWarning("is Grounde");
                grounded = true; // The car is above the ground
                if (!activateVFXPerGround)
                {

                    // activate the splash VFX
                    Debug.LogWarning("Activate VFX per ground");
                    activateVFXPerGround = true;
                }
                return grounded;
            }
        }else
        {
            grounded = false;
            activateVFXPerGround = false;
        }

       
        Debug.LogWarning("is NOT Grounded, is jump");
        return grounded;
        
    }


    // Move the player to the center of the Map (Whirlpool)
    public void MoveTowardCenter(Vector3 direction, float force)
    {
        _rigidbody.AddForce(direction * force * 20.0f, ForceMode.Force);

        //_carBodyTransform.Translate(direction * force * Time.deltaTime);
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("On Trigger Enter: " + other.name);
    //    WhirlpoolCollider whirlpoolCollider = other.GetComponent<WhirlpoolCollider>();
    //    if(whirlpoolCollider != null)
    //    {
    //        Debug.Log("Player Enter Whirlpool Collider");
    //        Managers.Instance.ultimateAbilityManager.isForceToCenter = false;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("On Trigger Exit: " + other.name);
    //    WhirlpoolCollider whirlpoolCollider = other.GetComponent<WhirlpoolCollider>();
    //    if (whirlpoolCollider != null)
    //    {
    //        Debug.Log("Player Exit Whirlpool Collider");
    //        if (coroutine != null)
    //        {
    //            coroutine = null;
    //        }else
    //        {
    //            coroutine = StartCoroutine(PullBackPlayerToWhirlpoolWithDelay(5.0f));
    //        }
    //    }
    //}

    //public IEnumerator PullBackPlayerToWhirlpoolWithDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    Managers.Instance.ultimateAbilityManager.isForceToCenter = true;
    //}
}
