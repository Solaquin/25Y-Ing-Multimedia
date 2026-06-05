using UnityEngine;

[CreateAssetMenu(fileName = "BuffSelfSO", menuName = "Combat/Effects/Buff Self Effect")]
public class BuffSelfEffectSO : MoveEffectSO
{
    public override void Execute(
    CombatUnit user,
    CombatUnit target,
    MoveContext context
)
    {
        bool changed = user.AddStageModifier(
            context.move.affectedStat,
            Mathf.Abs(context.move.stageChange)
        );

        if (changed)
        {
            int amount = Mathf.Abs(context.move.stageChange);

            string intensity =
                amount >= 2
                ? "drásticamente"
                : "";

            context.AddMessage(
                $"{user.Instance.data.professorName} aumentó {intensity} su {context.move.affectedStat}."
            );
        }
    }
}
