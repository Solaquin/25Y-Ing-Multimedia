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
        int healAmount = Mathf.RoundToInt(user.Instance.maxHP * (context.move.healPercent / 100f));
        user.Heal(healAmount);
    }
}