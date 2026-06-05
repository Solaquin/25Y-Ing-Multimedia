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
        bool changed = target.AddStageModifier(
            context.move.affectedStat,
            -Mathf.Abs(context.move.stageChange)
        );

        if (changed)
        {
            int amount = Mathf.Abs(context.move.stageChange);

            string intensity =
                amount >= 2
                ? "drásticamente"
                : "";

            context.AddMessage(
                $"{target.Instance.data.professorName} redujo {intensity} su {context.move.affectedStat}."
            );
        }
    }
}
