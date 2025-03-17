using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    void EnterState(EnemyAI enemyAI);
    void UpdateState();
    void ExitState();
    string GetStateName();
}
