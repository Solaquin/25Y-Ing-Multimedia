using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Status/Sleep")]
public class SleepStatusSO : StatusEffectSO
{
    public override bool PreventAction(BattleActionType actionType)
    {
        if (actionType == BattleActionType.Move)
            return true;

        return false;
    }

    public override void OnTurnEnd(CombatUnit unit)
    {
        Debug.Log($"{unit.name} está dormido...");
    }
}
