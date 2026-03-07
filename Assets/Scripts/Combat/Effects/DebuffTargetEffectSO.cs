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
        target.AddStageModifier(context.move.affectedStat, -Mathf.Abs(context.move.stageChange));
    }
}
