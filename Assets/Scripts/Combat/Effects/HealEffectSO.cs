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
        if (user.GetCurrentHP() >= user.GetMaxHP())
        {
            context.AddMessage("Pero no tuvo efecto.");
            return;
        }

        int healAmount =
            Mathf.RoundToInt(
                user.Instance.maxHP *
                (context.move.healPercent / 100f)
            );

        user.Heal(healAmount);

        context.AddMessage(
            $"¡{user.Instance.data.professorName} recuperó puntos de vida!"
        );
    }
}