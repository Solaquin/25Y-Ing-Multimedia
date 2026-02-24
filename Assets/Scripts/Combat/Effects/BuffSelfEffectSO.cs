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
        user.AddModifier(context.move.affectedStat, context.move.amount, context.move.duration);
    }
}
