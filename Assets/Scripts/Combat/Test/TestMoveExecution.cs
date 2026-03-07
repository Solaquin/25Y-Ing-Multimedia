using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TestMoveExecution : MonoBehaviour
{
    public CombatUnit player;
    public CombatUnit enemy;
    public MoveSO playerMove;
    public MoveSO enemyMove;
    public BattleSystem battleSystem;

    public bool makeTest = false;

    void Start()
    {
        if(makeTest)
        {
            battleSystem.allUnits.Add(player);
            battleSystem.allUnits.Add(enemy);

            battleSystem.RunBattleTest();
        }
    }
}