using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyAI : NetworkBehaviour, IDamage
{
    
    private IEnemyState currentState;

    [SerializeField]
    private string currentStateString;

  
    public EnemyType enemyType;


    public MeshRenderer playerRender;
    public Material playerBaseMaterial;
    public Material playerHitMaterial;

    [SerializeField]
    private HitEffect hitEffect;

    [Header("ENEMY HEALTH")]
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
            // hit effect start flashing
        }
    }
    public int maxHealth = 100;
    public int minHealth = 0;



    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        hitEffect = GetComponent<HitEffect>();
        ChangeState(new EnemyStates.IdleState());
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        currentState?.UpdateState();

        UpdateCurrentStateString();
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }


    void UpdateCurrentStateString()
    {
        if (currentState != null)
        {
            currentStateString = currentState.GetStateName();// Convert currentState to a string representation
        }
        else
        {
            currentStateString = "No State";
        }
    }

    public void TakeDamage(int amount, NetworkObject networkObject)
    {
        Debug.Log($"<color> Enemy AI Take Damage! : {amount}  </color>");
        Health -= amount;
        hitEffect.StartFlashEffect();
        if(Health <= 0)
        {
            Debug.Log($"<color=red> Enemy Die...  </color>");
        }
    }
}

public enum EnemyType
{
    crab,
    shark,
    turtle,
    giantCrab
}
