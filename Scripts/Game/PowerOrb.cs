using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PowerOrb : NetworkBehaviour, IInteractable
{
    [Networked]
    public int ManaPower { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        ManaPower = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetMana()
    {
        Debug.Log("Hit Orb");
        return ManaPower;
    }

    public void DespawnNetworkObject(Player player)
    {
        player.RpcUpdateManaUI(
            player.gameObject.GetComponent<NetworkObject>(),
            GetMana(),
            player.maxMana,
            player.minMana
            );
        Runner.Despawn(Object);
    }

    public void Interact(Player player)
    {
        DespawnNetworkObject(player);
    }
}
