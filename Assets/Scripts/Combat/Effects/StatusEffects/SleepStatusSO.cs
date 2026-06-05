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

    public override string GetApplyMessage(CombatUnit unit)
    {
        return $"{unit.Instance.data.professorName} se quedó dormido.";
    }

    public override string GetPreventActionMessage(CombatUnit unit)
    {
        return $"{unit.Instance.data.professorName} está dormido y no puede actuar.";
    }
}
