using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private Coroutine coroutinePull;
    private Coroutine coroutineStopPull;
    public Player player;

    private IInteractable _interactable = null;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On Trigger Enter: " + other.name);
        WhirlpoolCollider whirlpoolCollider = other.GetComponent<WhirlpoolCollider>();
        if (whirlpoolCollider != null)
        {
            Debug.Log("Player Enter Whirlpool Collider");
            //Managers.Instance.ultimateAbilityManager.isForceToCenter = false;
            if (coroutineStopPull != null)
            {
                coroutineStopPull = null;
                coroutineStopPull = StartCoroutine(StopPullbackPlayerWhirlPool(1.0f));
            }
            else
            {
                coroutineStopPull = StartCoroutine(StopPullbackPlayerWhirlPool(1.0f));
            }
        }

        //PowerOrb powerOrb = other.GetComponent<PowerOrb>();
        //if(powerOrb != null)
        //{
        //    // Get Mana Power
        //    // Activate Mana VFX
        //    powerOrb.DespawnNetworkObject();
        //}

        _interactable = other.GetComponent<IInteractable>();
        if(_interactable != null)
        {
            _interactable.Interact(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("On Trigger Exit: " + other.name);
        WhirlpoolCollider whirlpoolCollider = other.GetComponent<WhirlpoolCollider>();
        if (whirlpoolCollider != null)
        {
            Debug.Log("Player Exit Whirlpool Collider");
            if (coroutinePull != null)
            {
                coroutinePull = null;
                coroutinePull = StartCoroutine(PullBackPlayerToWhirlPoolWithDelay(0.1f));
            }
            else
            {
                coroutinePull = StartCoroutine(PullBackPlayerToWhirlPoolWithDelay(0.1f));
            }
        }
    }

    public IEnumerator PullBackPlayerToWhirlPoolWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.isForceToCenter = true;
    }

    public IEnumerator StopPullbackPlayerWhirlPool(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.isForceToCenter = false;
    }
}
