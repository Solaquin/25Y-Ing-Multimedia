using UnityEngine;

public class TestMoveExecution : MonoBehaviour
{
    public CombatUnit attacker;
    public CombatUnit defender;
    public MoveSO move;
    public BattleSystem battleSystem;

    private void Start()
    {
        battleSystem.UseMove(
            attacker,
            defender,
            move
        );
    }
}