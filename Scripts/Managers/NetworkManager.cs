using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using EditorButton;
using System.Threading.Tasks;
using System.Threading;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private string roomCode;

    public string RoomCode
    {
        get
        {
            return roomCode;
        }
    }

    [SerializeField]
    private NetworkRunner networkRunnerPrefab;

    /// <summary>
    /// NetworkRunner is a component of Fusion,
    /// it manages the entire netowrking stack
    /// including connection, input handle and object synchronization.
    /// it responsable for setting up and controlling the Game Network Behaviour
    /// </summary>
    private NetworkRunner _networkRunner;
    public NetworkRunner GetNetworkRunner
    {
        get
        {
            return _networkRunner;
        }
    }
    private CancellationTokenSource _cancellationTokenSource;
    private readonly int gameSceneIndex = 1;

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
        ;
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
       
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player join");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player left");
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

    public async Task StartGame(GameMode mode, string sessionCode, CancellationToken cancellationToken)
    {
        try
        {
            roomCode = sessionCode;
            //_networkRunner = gameObject.AddComponent<NetworkRunner>();
            //_networkRunner.ProvideInput = true;
            if (_networkRunner == null)
            {
                _networkRunner = Instantiate(networkRunnerPrefab, transform);
                _networkRunner.AddCallbacks(this);
                _networkRunner.ProvideInput = true;
            }
            // a boolea var,  if mode == gamemode.host is true, if mode != game.host is false.
            var tryingHost = mode == GameMode.Host;
            Debug.Log(tryingHost ? $"Starting as a host with code {roomCode}" : $"Starting as a join with code {roomCode}");

            var result = await _networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomCode,
                Scene = gameSceneIndex,//SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.GetComponent<NetworkSceneManagerBase>() //gameObject.AddComponent<NetworkSceneManagerBase>()

            });

            if (result.Ok)
            {
                Debug.Log(tryingHost ? "Game Started Successfully" : "Game Joined Successfully");
            }
            else
            {
                Debug.LogError("Game failed to " + (tryingHost ? "join" : "start") + " with result: " + result.ShutdownReason);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("StartGame operation was canceled");
            OnStartCanceled();
        }
        catch (Exception ex)
        {
            Debug.Log("An error ocurred during stargame: " + ex.Message);
        }
       

    }

    private void OnStartCanceled()
    {
        if (_networkRunner)
        {
            _networkRunner.Shutdown();
            _networkRunner = null;
        }
    }

    [Button()]
    private async Task StartAsHost()
    {
        CancellationTokenSource _cancellationTokenSource;
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        await StartGame(GameMode.Host, Extensions.SessionCodeGenerator.GenerateSessionCode(), cancellationToken);
    }

    [Button()]
    private async Task StartAsClient()
    {
        CancellationTokenSource _cancellationTokenSource;
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        await StartGame(GameMode.Client, Managers.Instance.lobbyUIManager.connectionPanelController.GetRoomCodeInputField, cancellationToken);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
