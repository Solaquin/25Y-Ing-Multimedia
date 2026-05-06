using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public StatusType statusType;

    // 🔊 AUDIO AL APLICAR EL ESTADO
    [Header("Audio")]
    public AudioClip sfxOnApply;

    public virtual void OnApply(CombatUnit unit) { }

    public virtual void OnTurnStart(CombatUnit unit) { }

    public virtual void OnTurnEnd(CombatUnit unit) { }

    public virtual void OnRemove(CombatUnit unit) { }

    public virtual bool PreventAction(BattleActionType actionType)
    {
        return false;
    }
}