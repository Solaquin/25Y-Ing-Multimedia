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
        target.ApplyStatus(context.move.statusEffect, context.move.duration);
    }
}
