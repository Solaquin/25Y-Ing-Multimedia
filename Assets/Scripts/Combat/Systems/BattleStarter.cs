using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public BattleSystem battleSystem;
    public NPCParty enemyParty;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            StartBattle();
        }
    }
    public void StartBattle()
    {
        battleSystem.SetupBattle(enemyParty);
    }
}