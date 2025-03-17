using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BulletNetwork : NetworkBehaviour
{
    [Networked]
    private TickTimer life { get; set; }

    private NetworkId playerSpawnerBulletId;
    public NetworkId PlayerSpawnerBulletId
    {
        get
        {
            return playerSpawnerBulletId;
        }
        set
        {
            playerSpawnerBulletId = value;
        }
    }

    public bool isFollowTarget = false;
    public Vector3 targetPosition;
    public int damage;

    private IDamage _iDamage = null;

    public LayerMask enemyTargetLayer;

    public void Init(NetworkId id, int takeDamage, bool follow, Vector3 targetPos)
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        PlayerSpawnerBulletId = id;
        damage = takeDamage;
        isFollowTarget = follow;
        targetPosition = targetPos;
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else if (isFollowTarget)
        {
            // lerp towards the target position smoothly
            float moveSpeed = 5.0f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Runner.DeltaTime * moveSpeed);
        }
        else
        {
            transform.position += 25 * transform.forward * Runner.DeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkObject colliderNetworkObject = other.transform.parent?.parent?.parent?.parent?.GetComponent<NetworkObject>();
        Player colliderPlayer = other.transform.parent?.parent?.parent?.parent?.GetComponent<Player>();

       


        if(colliderNetworkObject != null)
        {
            string userId = Runner.UserId;
            
            Debug.Log("ID:" + userId);

            string hitUserId = colliderNetworkObject.Runner.UserId;

            Debug.Log("Hit ID:" + hitUserId);

            //if (!colliderNetworkObject.HasInputAuthority)
            //{
            //    Debug.Log("Hit ID: " + colliderNetworkObject.Id);
            //}

            if(colliderNetworkObject.Id == playerSpawnerBulletId)
            {
                return;
            }else
            {
                Debug.Log(".....Hid ID: " + colliderNetworkObject.Id);

                Debug.Log("<color=red> HIT </color>");

                if (colliderPlayer != null)
                {

                    _iDamage = other.transform.parent?.parent?.parent?.parent?.GetComponent<IDamage>();

                    if (_iDamage != null)
                    {
                        _iDamage.TakeDamage(damage, colliderNetworkObject);
                    }
                    //colliderPlayer.Health -= damage; //1;

                    //colliderPlayer.RpcMessageToAllPlayers(colliderNetworkObject, "Hello World RPC");

                    //colliderPlayer.RpcUpdateHealthUI(
                    //     colliderNetworkObject,
                    //     colliderPlayer.Health,
                    //     colliderPlayer.maxHealth,
                    //     colliderPlayer.minHealth
                    //    );


                    //colliderPlayer.hitEffect.StartFlashEffect();
                    //colliderPlayer.cameraShake.ShakeCamera();
                    //Debug.Log("Hit and Damage a Enemy Health");

                    //if (colliderPlayer.Health <= 0)
                    //{
                    //    Debug.Log("Enemy Health is < 0: You win");
                    //    colliderPlayer.Die();
                    //}

                    //===================================

                    //colliderPlayer.statsView.RpcUpdateHealthUI(
                    //       colliderNetworkObject,
                    //       colliderPlayer.Health,
                    //       colliderPlayer.maxHealth,
                    //       colliderPlayer.minHealth
                    //   );


                    //Managers.Instance.SetPlayerHealth(
                    //    colliderPlayer.Health,
                    //    colliderPlayer.maxHealth,
                    //    colliderPlayer.minHealth
                    //    );

                    //colliderPlayer.statsView.SetHealthBarUI(
                    //        colliderPlayer.Health,
                    //        colliderPlayer.maxHealth,
                    //        colliderPlayer.minHealth
                    //    );


                    //colliderPlayer.gameUIManager.SetHealthBarUI(
                    //        colliderPlayer.Health,
                    //        colliderPlayer.maxHealth,
                    //        colliderPlayer.minHealth
                    //    );

                    // ========================================
                }


            }

            if (colliderNetworkObject.Runner.LocalPlayer.PlayerId !=  Runner.LocalPlayer.PlayerId)
            //Managers.Instance.networkManager.transform.GetChild(0).GetComponent<NetworkRunner>().LocalPlayer)
            {
                

            }

          
        }


        if((enemyTargetLayer.value & 1 << other.gameObject.layer) != 0)
        {
            Debug.Log($"<color> HIT ENEMY </color>");
            NetworkObject enemyAINetworkObject = other.transform.parent?.parent?.parent?.GetComponent<NetworkObject>();
            //EnemyAI enemyAI = other.transform.GetComponent<EnemyAI>();
            if (enemyAINetworkObject != null)
            {
                _iDamage = other.transform.parent?.parent?.parent?.GetComponent<IDamage>();
                if (_iDamage != null)
                {
                    _iDamage.TakeDamage(10, colliderNetworkObject);
                }
            }
        }
      

    }

    
}
