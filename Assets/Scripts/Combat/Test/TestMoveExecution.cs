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

    void Start()
    {
        battleSystem.allUnits.Add(player);
        battleSystem.allUnits.Add(enemy);

        if (player.GetMoves().Count == 0 || enemy.GetMoves().Count == 0)
        {
            Debug.LogError("Uno de los CombatUnit no tiene movimientos.");
            return;
        }

        MoveSO playerMove = player.GetRandomMove();
        MoveSO enemyMove = enemy.GetRandomMove();

        List<BattleCommand> commands = new List<BattleCommand>()
        {
            BattleCommand.CreateMoveCommand(player, enemy, playerMove),
            BattleCommand.CreateMoveCommand(enemy, player, enemyMove)
        };

        battleSystem.ResolveTurn(commands);
    }
}