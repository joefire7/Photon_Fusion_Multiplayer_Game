using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Fusion;
using UnityEngine.UI;

public class StatsView : MonoBehaviour
{

    [Header("Health / Mana Bar UI")]
    public Image healthBarUI;
    public Image manaBarUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void DebugMessage(string message)
    {
        Debug.Log(message);
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    //public void RpcUpdateHealthUI(NetworkObject id, int currentHealth, int maxHealth, int minHealth, RpcInfo info = default)
    //{
    //    //if (info.IsInvokeLocal)
    //    //{
    //    //    Debug.Log("RPC: MESSAGE TO ALL PLAYER: " + id);

    //    //}else
    //    //{
    //    //    Debug.Log("RPC: MESSAGE TO ALL PLAYER: " + id);
    //    //}
    //    RPC_RelayMessage(id, info.Source);
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    //public void RPC_RelayMessage(NetworkObject id, PlayerRef messageSource)
    //{
    //    //if (_messages == null)
    //    //    _messages = FindObjectOfType<TMP_Text>();

    //    if (messageSource == Runner.LocalPlayer)
    //    {
    //        Debug.Log($"You said: {id}\n");
    //    }
    //    else
    //    {
    //        Debug.Log($"Some other player said: {id}\n");
            
    //    }

        
    //}


    public void SetHealthBarUI(int currentHealth, int maxHealth, int minHealth)
    {
        //if (HasStateAuthority)
        //{
        float normalized = NormalizedFloatValue(currentHealth, maxHealth, minHealth);
        healthBarUI.fillAmount = normalized;
       // }

    }
    public void SetManaBarUI(int currentMana, int maxMana, int minMana)
    {
        //if (HasStateAuthority)
        //{
        float normalized = NormalizedFloatValue(currentMana, maxMana, minMana);
        manaBarUI.fillAmount = normalized;
        // }

    }

    // Convert Int to Float and then Normalized those value between (0, 1)
    public float NormalizedFloatValue(int currentValue, int maxValue, int minValue)
    {
        float normalized = (float)(currentValue - minValue) / (maxValue - minValue);
        return normalized;
    }





}
