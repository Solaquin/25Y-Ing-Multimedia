using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Status/Paralysis")]
public class ParalysisStatusSO : StatusEffectSO
{
    public float skipChance = 0.25f;

    public override bool PreventAction(BattleActionType actionType)
    {
        if (actionType != BattleActionType.Move)
            return false;

        return Random.value < skipChance;
    }

    public override string GetApplyMessage(CombatUnit unit)
    {
        return $"{unit.Instance.data.professorName} se bloqueo creativamente.";
    }

    public override string GetPreventActionMessage(CombatUnit unit)
    {
        return $"¡El bloqueo creativo impidió actuar a {unit.Instance.data.professorName}!";
    }
}
