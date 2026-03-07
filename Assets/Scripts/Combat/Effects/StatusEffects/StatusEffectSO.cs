using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public StatusType statusType;

    public virtual void OnApply(CombatUnit unit) { }

    public virtual void OnTurnStart(CombatUnit unit) { }

    public virtual void OnTurnEnd(CombatUnit unit) { }

    public virtual bool PreventAction()
    {
        return false;
    }
}