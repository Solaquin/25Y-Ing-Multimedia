using UnityEngine;

[CreateAssetMenu(fileName = "HealEffectSO", menuName = "Combat/Effects/Heal Effect")]
public class HealEffectSO : MoveEffectSO
{
    public override void Execute(
    CombatUnit user,
    CombatUnit target,
    MoveContext context
)
    {
        user.Heal(context.move.healAmount);
    }
}
