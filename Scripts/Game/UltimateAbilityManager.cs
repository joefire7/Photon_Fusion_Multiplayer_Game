using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class UltimateAbilityManager : MonoBehaviour
{
    [SerializeField]
    private GameObject whirlpool;
    [SerializeField]
    private GameObject crab;

    [Header("Whirlpool Ability to pull every Network Players to the center of the map")]
    public float pullRadio = 40.0f;
    public float pullForce = 5.0f;
    public bool letPullPlayersToCenter;
    public bool isForceToCenter;
    public LayerMask targetLayer;

    public Transform testCenterPos;
    private Transform _ultimateAbilityManagerTrans;
    public Transform UltimateAbilityManagerTrans
    {
        get
        {
            return _ultimateAbilityManagerTrans;
        }
    }

    public Vector3 GetPosition
    {
        get
        {
            return transform.position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        letPullPlayersToCenter = false;
        if(_ultimateAbilityManagerTrans == null)
        {
            _ultimateAbilityManagerTrans = GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (letPullPlayersToCenter)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, pullRadio);

           
            foreach (Collider col in colliders)
            {

                //Player player = col.GetComponent<Player>();
                Player player = col.transform.parent?.parent?.parent?.parent?.GetComponent<Player>();

                // Draw wire sphere around each detected object
                DebugDrawOverlapSphere(col.transform.position, pullRadio);

                if (player != null)
                {
                    Debug.Log("Player OverlapSphere is not null");
                    Vector3 direction = transform.position - col.transform.position;
                    player.simpleCarController.MoveTowardCenter(direction.normalized, pullForce * Time.deltaTime);
                }else
                {
                    Debug.Log("Player OverlapSphere is Null");
                }
            }
        }
    }

    public void Init()
    {

    }


    // Debug method to draw the wire sphere in the Scene view
    void DebugDrawOverlapSphere(Vector3 center, float radius)
    {
        // Draw the wire sphere
        for (int i = 0; i < 360; i += 30)
        {
            Vector3 pos = center + new Vector3(Mathf.Cos(i * Mathf.Deg2Rad) * radius, 0, Mathf.Sin(i * Mathf.Deg2Rad) * radius);
            Vector3 nextPos = center + new Vector3(Mathf.Cos((i + 30) * Mathf.Deg2Rad) * radius, 0, Mathf.Sin((i + 30) * Mathf.Deg2Rad) * radius);

            Debug.DrawLine(pos, nextPos, Color.red);
        }
    }
}
