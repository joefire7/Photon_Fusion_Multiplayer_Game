using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour, IDamage
{
    // Fixed Update Network, [Networked], RPC (Remote Procedual Call)

    [SerializeField] private SimpleCarController carController;

    [SerializeField] private GameObject localComponent;

    [SerializeField] private CameraController cameraController;
    public CameraController CameraController
    {
        get
        {
            return cameraController;
        }

        set
        {
            cameraController = value;
        }
    }

    [Networked] public CarInputData InputData { get; set; }

    public SimpleCarController simpleCarController;

    public MeshRenderer playerRender;
    public Material playerBaseMaterial;
    public Material playerHitMaterial;


    public HitEffect hitEffect;

    [Header("PLAYER HEALTH")]
    [SerializeField]
    private int health;
    [Networked] public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            hitEffect.StartFlashEffect();
        }
    }

    public int maxHealth = 100;
    public int minHealth = 0;

    [Header("PLAYER MANA")]
    [SerializeField]
    private int mana;
    [Networked] public int Mana
    {
        get
        {
            return mana;
        }
        set
        {
            mana = value;
        }
    }

    public int maxMana = 100;
    public int minMana = 0;

    [Header("Rules of the Game")]
    public bool isForceToCenter;

    private CenterField centerField;
    public StatsView statsView;

    [Header("Cameras")]
    public CameraShake cameraShake;

    // On Fusion Networking Spawned, check if this object have input authority to use the local components
    // if not! let destroy the local components, this will prevent to other player use the same inputs
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            localComponent.SetActive(true);

            Managers.Instance.playerRef = Object.InputAuthority;
            Managers.Instance.networkId = Object.Id;
            Managers.Instance.networkSpawnerController.playerSpawnerNetworkId = Object.Id;
        }
        else
        {
            localComponent.SetActive(false);
        }

        Health = maxHealth;
        health = 100;
        hitEffect = GetComponent<HitEffect>();
        centerField = FindAnyObjectByType<CenterField>();
        statsView = FindAnyObjectByType<StatsView>();
        mana = 5;
    }

    public override void FixedUpdateNetwork()
    {
        //if (Object.HasInputAuthority)
        //{
            if(GetInput(out CarInputData data))
            {
            InputData = data;


            //carController.SetInputData(data);
            }
        //}
            carController.SetInputData(InputData);
    }

    public void SetGameBall(Transform ballVisual)
    {
        cameraController.SetGameBall(ballVisual);
    }

    public void Die()
    {
        if(centerField != null)
        {
            // Player Respawn
            Health = 100;
            simpleCarController.playerTransform.position = centerField.GetComponent<Transform>().position;
        }

       
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcMessageToAllPlayers(NetworkObject networkObject, string rpcMessage, RpcInfo info = default)
    {
        Debug.Log("RPC Message: " + rpcMessage);
        Debug.Log("RPC hit object network object id: " + networkObject.Id);

        //if (networkObject.Id != simpleCarController.playerNetworkObject)
        //{

        //}playerNetworkObject
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcUpdateHealthUI(NetworkObject networkObject, int currentHealth, int maxHealth, int minHealth, RpcInfo info = default)
    {
        //Debug.Log("RPC Message: " + rpcMessage);
        //Debug.Log("RPC hit object network object id: " + networkObject.Id);
        
        Debug.Log("Hit Network ID : " + networkObject.Id + " " + "current player networkd id: " + simpleCarController.playerNetworkObject.Id);

        if (networkObject.Id == simpleCarController.playerNetworkObject.Id)
        {
            if (networkObject.HasInputAuthority)
            {
                Debug.Log("RPC Update Health UI from ID: " + networkObject.Id);
                statsView.SetHealthBarUI(
                    currentHealth,
                    maxHealth,
                    minHealth
                    );
            }
           
        }
        
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcUpdateManaUI(NetworkObject networkObject, int currentMana, int maxMana, int minMana, RpcInfo info = default)
    {
        //Debug.Log("RPC Message: " + rpcMessage);
        //Debug.Log("RPC hit object network object id: " + networkObject.Id);
        Mana += currentMana;
        Debug.Log("Hit Network ID : " + networkObject.Id + " " + "current player networkd id: " + simpleCarController.playerNetworkObject.Id);

        if (networkObject.Id == simpleCarController.playerNetworkObject.Id)
        {
            if (networkObject.HasInputAuthority)
            {
                Debug.Log("RPC Update Mana UI from ID: " + networkObject.Id);
                statsView.SetManaBarUI(
                    Mana,
                    maxMana,
                    minMana
                    );
            }

        }

    }

    public void TakeDamage(int amount, NetworkObject networkObject)
    {
        Debug.Log($"Player Take Damage: {amount}");
        Health -= amount;
        RpcMessageToAllPlayers(networkObject, "Hello World RPC");
        RpcUpdateHealthUI(
            networkObject,
            Health,
            maxHealth,
            minHealth
            );
        hitEffect.StartFlashEffect();
        cameraShake.ShakeCamera();
        Debug.Log("Hit and Damage a Player Health");

        if(Health <= 0)
        {
            Die();
        }
    }
}
