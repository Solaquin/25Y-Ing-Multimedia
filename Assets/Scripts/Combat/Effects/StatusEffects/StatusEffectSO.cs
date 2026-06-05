using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public StatusType statusType;

    public virtual void OnApply(CombatUnit unit) { }

    public virtual void OnTurnStart(CombatUnit unit) { }

    public virtual void OnTurnEnd(CombatUnit unit) { }

    public virtual void OnRemove(CombatUnit unit) { }

    public virtual bool PreventAction(BattleActionType actionType)
    {
        return false;
    }

    public virtual string GetApplyMessage(CombatUnit unit)
    {
        return $"{unit.Instance.data.professorName} quedó {statusType}.";
    }

    public virtual string GetPreventActionMessage(CombatUnit unit)
    {
        return $"{unit.Instance.data.professorName} no puede actuar.";
    }
}