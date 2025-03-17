using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField]
    private NetworkPrefabRef gameBallNetworkPrefab;

    [SerializeField]
    private NetworkPrefabRef bulletNetworkPrefab;

    [SerializeField]
    private NetworkPrefabRef playerNetworkPrefab;

    [SerializeField, Range(0f, 10f)]
    private float randomSpawnPositionRange = 5f;

    [SerializeField]
    private List<NetworkObject> spawnedBallObjects = new List<NetworkObject>();

    private Dictionary<PlayerRef, NetworkObject> _cubesPlayers = new Dictionary<PlayerRef, NetworkObject>();

    private NetworkObject _ball;

    private Transform _ballVisual;

    [SerializeField]
    private Transform _playerPos;
    [SerializeField]
    private PlayerRef currentPlayerPref;

    public Transform currentSpawnPlayerTransform;

    public NetworkId playerSpawnerNetworkId;

    private CenterField centerField;

    // callback
    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            SpawnGameBall();
        }

        //Managers.Instance.GenericShootEvent += SpawnBullet;
    }

    public void SpawnGameBall()
    {
        centerField = FindAnyObjectByType<CenterField>();
        if(centerField != null)
        {

        var pos = centerField.GetComponent<Transform>().position; //new Vector3(0f, 2.0f, 0f);
        var _ball = Runner.Spawn(gameBallNetworkPrefab, pos, Quaternion.identity);
        _ballVisual = _ball.transform.GetChild(0).transform;

        }
    }

   

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    SpawnBall();
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    DespawnBalls();
        //}
    }

    // Spawn Ball 
    private void SpawnBall()
    {
        if (Runner.IsServer)
        {
            var randomPos = new Vector3(Random.Range(-randomSpawnPositionRange, randomSpawnPositionRange), 8.0f, Random.Range(-randomSpawnPositionRange, randomSpawnPositionRange));
            var ballNetworkObject = Runner.Spawn(gameBallNetworkPrefab, randomPos, Quaternion.identity);
            spawnedBallObjects.Add(ballNetworkObject);
        }
    }

    // DeSpawn Ball
    private void DespawnBalls()
    {
        if (Runner.IsServer)
        {
            foreach (var ball in spawnedBallObjects)
            {
                Runner.Despawn(ball);
            }
        }
    }

    // callback
    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            SpawnPlayerObject(player);
        }
    }

    // Spawn Cube Network Object
    public void SpawnPlayerObject(PlayerRef player)
    {
       
            currentPlayerPref = player;
            Vector3 defaultPos = Vector3.zero;
            //var randomPos = new Vector3(Random.Range(-randomSpawnPositionRange, randomSpawnPositionRange), 2.0f, Random.Range(-randomSpawnPositionRange, randomSpawnPositionRange));
            var spawnPlayer = Runner.Spawn(playerNetworkPrefab, defaultPos, Quaternion.identity, player);

            currentSpawnPlayerTransform = spawnPlayer.transform.GetChild(1).GetChild(0).GetChild(0);

            spawnedBallObjects.Add(spawnPlayer);
            _cubesPlayers.Add(player, spawnPlayer);

            var playerScript = spawnPlayer.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.SetGameBall(_ballVisual);
                _playerPos = playerScript.transform;
            }
        
      
    }

    // cabllback
    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            DeSpawnPlayerObject(player);
        }
    }

    // DeSpawn Cube Network Object
    public void DeSpawnPlayerObject(PlayerRef player)
    {
        if(_cubesPlayers.TryGetValue(player, out var demoObject))
        {
            Runner.Despawn(demoObject);
            _cubesPlayers.Remove(player);
        }
    }


    // Shoot bullet
    public void SpawnBullet(Transform playerPos, NetworkObject playerNetworkObject)
    {
        //NetworkManager networkManager = new NetworkManager();
        //NetworkObject localPlayer = networkManager.

            Vector3 playerForward = playerPos.forward;
            Quaternion bulletForward = Quaternion.LookRotation(playerForward);

            float yOffset = 1.0f; // Change this value to adjust the upward offset
            float forwardOffset = 5.0f; // Change this value to adjust the forward offset

            Vector3 spawnPosition = playerPos.position + playerForward * forwardOffset + Vector3.up * yOffset;

            Transform pos = currentSpawnPlayerTransform;//GameObject.Find("PredictedObj").GetComponent<Transform>()
            var bullet = Runner.Spawn(bulletNetworkPrefab, spawnPosition, bulletForward);

            BulletNetwork bulletNetwork = bullet.GetComponent<BulletNetwork>();
            if (bulletNetwork != null)
            {
                bulletNetwork.Init(playerNetworkObject.Id, 10, false, Vector3.zero);
            }else
            {
            Debug.Log("<color=red> Bullet Network is Null... </color>");
            }

            //Rigidbody rb = bullet.GetComponent<Rigidbody>();
            //if (rb != null)
            //{

            //    Vector3 fowardOffset = pos.forward * 1.5f;
            //    Vector3 upwardOffset = pos.up * 2.0f;
            //    Vector3 combineOffset = fowardOffset + upwardOffset;
            //    bullet.transform.position += combineOffset;

            //    rb.velocity = pos.forward * 100f; // forward * bullet speed
            //}
            //else
            //{
            //    Debug.LogError("Rigidbody did not found...");
            //}
        
        
       
    }

}
