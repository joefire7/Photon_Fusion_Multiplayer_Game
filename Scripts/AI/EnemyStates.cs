using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyStates: NetworkBehaviour
{

    public float radioDetection = 5.0f;
    public bool canShoot = true;
    public List<Player> playersInRange = new List<Player>();

    [Header("Enemy Network Bullet")]
    [SerializeField]
    private NetworkPrefabRef bulletNetworkPrefab;

    [Header("Network Object")]
    [SerializeField]
    private NetworkObject _enemyNetworkObject;

    private EnemyAI _enemyAI;

    private Coroutine _coroutine;

    [SerializeField]
    private Animator _enemyAnimator;
    [SerializeField]
    private NetworkMecanimAnimator _networkAnimator;

    private void Start()
    {
        _enemyNetworkObject = GetComponent<NetworkObject>();
        _enemyAI = GetComponent<EnemyAI>();
        //_enemyAnimator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
        GetComponent<SphereCollider>().radius = radioDetection;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.LogWarning("On Trigger Stay.... With Enemy States");
        Player player = other.transform.parent?.parent?.parent?.parent?.GetComponent<Player>();
        
        if (player != null && !playersInRange.Contains(player))
        {
            player.CameraController.EnemyDynamicCamera(true);
            playersInRange.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.transform.parent?.parent?.parent?.parent?.GetComponent<Player>();
        if (player != null && playersInRange.Contains(player))
        {
            player.CameraController.EnemyDynamicCamera(false);
            playersInRange.Remove(player);
        }
    }

    public class PatrolState : NetworkBehaviour, IEnemyState
    {
        public void EnterState(EnemyAI enemyAI)
        {
            throw new System.NotImplementedException();
        }


        public void UpdateState()
        {
            throw new System.NotImplementedException();
        }

        public void ExitState()
        {
            throw new System.NotImplementedException();
        }

        public string GetStateName()
        {
            throw new System.NotImplementedException();
        }
    }
    public class IdleState : NetworkBehaviour, IEnemyState
    {
        private EnemyStates enemyStates;
        private float waitTime;

        public void EnterState(EnemyAI enemyAI)
        {
            Debug.Log("Start Idle State.... let wait the idle seconds");
            enemyStates = FindObjectOfType<EnemyStates>();
            waitTime = 5.0f;
            //enemyStates._enemyAnimator.Play("idle");
            enemyStates._networkAnimator.Animator.Play("idle");
        }


        public void UpdateState()
        {
            if (enemyStates != null)
            {
                if (enemyStates.canShoot == true && enemyStates.playersInRange.Count > 0)
                {

                    waitTime -= 1.0f * Managers.Instance.networkManager.GetNetworkRunner.DeltaTime;
                    waitTime = Mathf.Clamp(waitTime, 0f, 5.0f);
                    if(waitTime <= 0)
                    {
                       enemyStates._enemyAI.ChangeState(new EnemyStates.AttackState());
                    }
                }
            }
        }

        public void ExitState()
        {
            Debug.Log("Exit Idle State");
        }

        public string GetStateName()
        {
            return "IdleState";
        }

    }

    public class AttackState : NetworkBehaviour, IEnemyState
    {
        private EnemyStates enemyStates;
        private bool isAttackState = false;

        public void EnterState(EnemyAI enemyAI)
        {
            Debug.Log("Start Attack State");
            enemyStates = FindObjectOfType<EnemyStates>();
            enemyStates._coroutine = null;
            isAttackState = false;
        }

        public void UpdateState()
        {
            Debug.Log("UPDATE STATE: ATTACK STATE");
            Debug.Log(enemyStates.canShoot.ToString());
            Debug.Log("Player in range count: " + enemyStates.playersInRange.Count);
            if(enemyStates != null)
            {
                if (enemyStates.canShoot == true && enemyStates.playersInRange.Count > 0)
                {
                    //Vector3 randomPlayer = enemyStates.playersInRange[Random.Range(0, enemyStates.playersInRange.Count)].simpleCarController.playerTransform.position;
                    Transform randomPlayer = enemyStates.playersInRange[Random.Range(0, enemyStates.playersInRange.Count)].simpleCarController.playerTransform;
                    Debug.Log("AI Attack State: " + randomPlayer);
                    // Shoot Projectile towards the random player

                    if (!isAttackState)
                    {
                        EnemySpawnBullet(randomPlayer);
                        //enemyStates._enemyAnimator.Play("Combat_Unarmed_Attack01");
                        enemyStates._networkAnimator.Animator.Play("Combat_Unarmed_Attack01");
                    }
                    enemyStates._enemyAI.ChangeState(new EnemyStates.IdleState());
                }else if(enemyStates.playersInRange.Count <= 0)
                {
                    // next update will be to Patrol State(), let the enemy walk in the tiny island
                    enemyStates._enemyAI.ChangeState(new EnemyStates.IdleState());
                }
            }
           
        }
        public void ExitState()
        {
            Debug.Log("Exit Attack State");
        }

        public string GetStateName()
        {
            return "Attack State";
        }

        public void EnemySpawnBullet(Transform randomPlayerTransform)
        {
            isAttackState = true;
            Vector3 enemyFoward = enemyStates.gameObject.transform.forward;
            Quaternion bulletFoward = Quaternion.LookRotation(enemyFoward);
            float yOffset = 2.0f;
            float fowardOffset = 0.0f;
            Vector3 enemySpawnBulletPosition = enemyStates.gameObject.transform.position + enemyFoward * fowardOffset + Vector3.up * yOffset;
            var bullet = Managers.Instance.networkManager.GetNetworkRunner.Spawn
                (
                enemyStates.bulletNetworkPrefab,
                enemySpawnBulletPosition,
                bulletFoward
                );

            //StartCoroutine(MoveEnemyBulletTowardPlayer(bullet.transform, randomPlayerTransform.position));
            BulletNetwork bulletNetwork = bullet.GetComponent<BulletNetwork>();
            if (bulletNetwork != null)
            {
                bulletNetwork.Init(enemyStates._enemyNetworkObject.Id, 10, true, randomPlayerTransform.position);
            }else
            {
                Debug.Log("Bullet Network is NULL");
            }
        }

        // Coroutine to gradually move the bullet towards the random player position
        private IEnumerator MoveEnemyBulletTowardPlayer(Transform bulletTransform, Vector3 targetPosition)
        {
            Debug.Log("MoveEnemyBulletTowardPlayer.... ");
            float bulletSpeed = 2.0f;
            float distance = Vector3.Distance(bulletTransform.position, targetPosition);
            float jorneyLength = distance / bulletSpeed;
            float startTime = Time.time;

            while(Time.time < startTime + jorneyLength)
            {
                float distanceCovered = (Time.time - startTime) * bulletSpeed;
                float fracJourneyTime = distanceCovered / distance;
                bulletTransform.position = Vector3.Lerp(bulletTransform.position, targetPosition, fracJourneyTime);
                yield return null;
            }

            // ensure bullet reaches the exact target position
            bulletTransform.position = targetPosition;
            

            // handle any hit or destroy logic here
        }


    }

    public class HitState : NetworkBehaviour, IEnemyState
    {
        private EnemyStates enemyStates;
        private float waitTime;
        private float flashDuration = 0.1f;
        private Color flashColor = Color.red;
        private float flashTimer = 0f;
        private Renderer enemyRender;
        private Material enemyBaseMaterial;
        private Material enemyHitMaterial;
        private MaterialPropertyBlock enemyMaterialProps;
        private bool isHitState = false;

        public void EnterState(EnemyAI enemyAI)
        {
            enemyStates = FindObjectOfType<EnemyStates>();
            waitTime = 0.5f;

            if (!isHitState)
            {
                flashTimer = flashDuration;
            }
            
            enemyMaterialProps = new MaterialPropertyBlock();
            enemyRender.material = enemyBaseMaterial;
            
        }

        public void UpdateState()
        {
            if(flashTimer > 0)
            {
                isHitState = true;
                enemyRender.material = enemyHitMaterial;
                flashTimer -= Managers.Instance.networkManager.GetNetworkRunner.DeltaTime;
                float lerpValue = Mathf.PingPong(Runner.DeltaTime * 10.0f, flashDuration) / flashDuration;
                Color lerpedColor = Color.Lerp(Color.white, flashColor, lerpValue);
                // Set Shader Properties fot the Flash Effect
                enemyMaterialProps.SetColor("_FlashColor", lerpedColor);
                enemyMaterialProps.SetFloat("_FlashAmount", 1.0f);
                // Apply the modified properties to the renderer
                enemyRender.SetPropertyBlock(enemyMaterialProps);
                //enemyStates._enemyAnimator.Play("Combat_Unarmed_Hit");
                enemyStates._networkAnimator.Animator.Play("Combat_Unarmed_Hit");

            }
            else
            {
                waitTime -= 1.0f * Managers.Instance.networkManager.GetNetworkRunner.DeltaTime;
                waitTime = Mathf.Clamp(waitTime, 0f, 0.5f);
                if (waitTime <= 0)
                {
                    isHitState = false;
                    enemyMaterialProps.SetFloat("_FlashAmount", 0f);
                    enemyRender.SetPropertyBlock(enemyMaterialProps);
                    enemyRender.material = enemyBaseMaterial;
                    //enemyStates._enemyAnimator.Play("idle");
                    enemyStates._enemyAI.ChangeState(new EnemyStates.IdleState());
                }
               
            }
           
        }

        public void ExitState()
        {
            
        }

        public string GetStateName()
        {
            return "Hit State";
        }

    }
}
