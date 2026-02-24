using UnityEngine;

[CreateAssetMenu(fileName = "DebuffTargetEffectSO", menuName = "Combat/Effects/Debuff Target Effect")]
public class DebuffTargetEffectSO : MoveEffectSO
{

    public override void Execute(
        CombatUnit user,
        CombatUnit target,
        MoveContext context
    )
    {
        target.AddModifier(context.move.affectedStat, -context.move.amount, context.move.duration);
    }
}
