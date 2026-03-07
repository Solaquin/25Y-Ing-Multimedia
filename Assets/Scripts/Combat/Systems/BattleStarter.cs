using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public BattleSystem battleSystem;
    public NPCParty enemyParty;

    public void StartBattle()
    {
        battleSystem.SetupBattle(enemyParty);
    }
}