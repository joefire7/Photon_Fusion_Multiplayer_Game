using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class PlayerInputController : NetworkBehaviour, INetworkRunnerCallbacks
{
    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";
    private float horizontalValue;
    private float verticalValue;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Runner.AddCallbacks(this);
        }
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        FloatingJoystick floatingJoyStick = GameObject.FindWithTag("FloatingJoyStick").GetComponent<FloatingJoystick>();
        float horizontal;
        float vertical;
        if (floatingJoyStick != null)
        {
            horizontal = floatingJoyStick.Horizontal;
            vertical = floatingJoyStick.Vertical;

            horizontalValue = horizontal;//new GameUIManager().Instance.floatingJoyStick.Horizontal;//Input.GetAxis(horizontal);
            verticalValue = vertical;//new GameUIManager().Instance.floatingJoyStick.Vertical;//Input.GetAxis(vertical);

            Debug.LogWarning("Horizon Value: " + horizontalValue + " - " + "Vertical Value: " + verticalValue);
            Debug.LogWarning("Horizon JoyStick Value: " + horizontal + " - " + "Vertical JoyStick Value: " + vertical);
        }



        var data = new CarInputData()
        {
            Direction = new Vector3(horizontalValue, 0, verticalValue),
            IsBraking = Input.GetKey(KeyCode.Space),
            IsRocketing = Input.GetMouseButton(1),
            IsJumping = Managers.Instance.gameUIManager.isJumpingButtonPress,
            IsShooting = Managers.Instance.gameUIManager.isAttackButtonpPress,
            IsForceToCenter = player.isForceToCenter
            
        };

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

      
    
}