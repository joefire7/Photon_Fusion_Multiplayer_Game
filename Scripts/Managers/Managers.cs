using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

// Singleton
public class Managers : MonoBehaviour
{

    public static Managers Instance => _instacne;
    private static Managers _instacne;

    public NetworkManager networkManager;
    public NetworkSpawnerController networkSpawnerController;
    public LobbyUIManager lobbyUIManager;
    public GameUIManager gameUIManager;
    public UltimateAbilityManager ultimateAbilityManager;
    public UIManager uIManager;

    // public event Action<int> // public event Action<Bool> // Action<Vector3>
    // if i want to creatte events with parameter
    public event Action GenericShootEvent;
    public event Action GenericJumpEvent;
    public event Action<int, int, int> UpdatePlayerHealthEvent;

    public PlayerRef playerRef;

    public NetworkId networkId;

    public GameStateMode gameStateMode;

    private void Awake()
    {
        if(_instacne != null)
        {
            Destroy(gameObject);
            return;
        }

        _instacne = this;

        DontDestroyOnLoad(gameObject);

        
    }

    private void Start()
    {
        gameStateMode = GameStateMode.RecolectiveMode;
        // Load the GameUIManager reference every time a new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;

        
    }


    public void ShootPressButton()
    {
        //GenericShootEvent?.Invoke();
    }

    public void JumpPressButton()
    {
        GenericJumpEvent?.Invoke();
    }

    public void StartGame()
    {

    }

    public void GameOver()
    {

    }

    public void SetPlayerHealth(int currentHealth, int maxHealth, int minHealth)
    {
        UpdatePlayerHealthEvent?.Invoke(currentHealth, maxHealth, minHealth);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the GameObject with the "GameUI" tag in the newly loaded scene
        networkSpawnerController = GameObject.FindAnyObjectByType<NetworkSpawnerController>();
        gameUIManager = GameObject.FindAnyObjectByType<GameUIManager>();
        ultimateAbilityManager = FindAnyObjectByType<UltimateAbilityManager>();
        uIManager = FindAnyObjectByType<UIManager>();
    }

    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    public enum GameStateMode
    {
       RecolectiveMode,
       OffensiveDeffensiveMode,
       JungleBossMode,
       TriangleBermudaMode
    }
}



