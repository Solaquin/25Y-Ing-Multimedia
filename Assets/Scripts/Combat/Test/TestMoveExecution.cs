using System.Collections.Generic;
using UnityEngine;

public class TestMoveExecution : MonoBehaviour
{
    public CombatUnit player;
    public CombatUnit enemy;
    public MoveSO playerMove;
    public MoveSO enemyMove;
    public BattleSystem battleSystem;

    private void Start()
    {
        List<TurnAction> actions = new List<TurnAction>(){
            new TurnAction(player, enemy, playerMove),
            new TurnAction(enemy, player, enemyMove)
        };

        battleSystem.ExecuteTurn(actions);
    }
}