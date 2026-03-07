using UnityEngine;

public class TestInitalizeBattle : MonoBehaviour
{
    public BattleSystem battleSystem;

    public CombatUnit player;
    public CombatUnit enemy;

    void Start()
    {
        InitializeBattle();
    }

    void InitializeBattle()
    {
        battleSystem.allUnits.Clear();

        battleSystem.allUnits.Add(player);
        battleSystem.allUnits.Add(enemy);

        battleSystem.StartBattle();
    }
}
