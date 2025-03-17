using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PoolNetworkObjects : NetworkBehaviour
{
    [SerializeField]
    private NetworkPrefabRef OrbNetworkPrefab;

    [SerializeField]
    private float sphereRadius = 10.0f; // Radius of the sphere

    [SerializeField]
    private int numberOfObjects = 10; // Number of object to spawn

    [SerializeField]
    private NetworkPrefabRef enemyAINetworkPrefab;
    [SerializeField]
    private Transform enemyAISpawnPoint;

    public override void Spawned()
    {
        
        if (Runner.IsServer)
        {
            SpawnOrbs();
            SpawnEnemyType(EnemyType.crab);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnOrbs()
    {
        // Spawn Objects inside the sphere
        for (int i = 0; i < numberOfObjects; i++)
        {
            //Vector3 pos = new Vector3(0, 0, 0);

            // Generate random position inside a sphere
            Vector3 randomPos = Random.insideUnitSphere * sphereRadius;
            var orb = Runner.Spawn(OrbNetworkPrefab, randomPos, Quaternion.identity);
            orb.gameObject.SetActive(false);
            orb.gameObject.SetActive(true);
        }
    }

    public void SpawnEnemyType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.crab:
                // Spawn a Crab
                Vector3 rot = new Vector3(18.0f, 218.0f, 0f);
                Quaternion quaternion = Quaternion.Euler(rot);
                var enemy = Runner.Spawn(enemyAINetworkPrefab, enemyAISpawnPoint.position, quaternion);
            break;
            default:
                break;
        }
    }
}
