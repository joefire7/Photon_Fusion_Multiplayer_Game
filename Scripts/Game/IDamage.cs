using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public interface IDamage
{
    public void TakeDamage(int amount, NetworkObject networkObject);
}
