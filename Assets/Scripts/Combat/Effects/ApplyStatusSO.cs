using System.ComponentModel.Design.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplyStatusSO", menuName = "Combat/Effects/Apply Status Effect")]
public class ApplyStatusSO : MoveEffectSO
{
    public override void Execute(
    CombatUnit user,
    CombatUnit target,
    MoveContext context)
    {
        bool applied = target.ApplyStatus(
            context.move.statusEffect,
            context.move.duration
        );

        if (applied)
        {
            context.AddMessage(context.move.statusEffect.GetApplyMessage(target));
        }
        else
        {
            context.AddMessage("Pero no tuvo efecto.");
        }
    }
}
