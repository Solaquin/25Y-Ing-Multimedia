using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffectSO", menuName = "Combat/Effects/Damage Effect")]
public class DamageEffectSO : MoveEffectSO
{
    public override void Execute(CombatUnit user, CombatUnit target, MoveContext context)
    {
        int attack = user.GetStat(StatType.Attack);

        int defense = target.GetStat(StatType.Defense);

        if (defense < 0)
        {
            defense = 1;
        }

        float typeMultiplier = target.GetTypeMultiplier(context.move.moveType);

        float rawDamage = context.move.power * ((float)attack / defense) * typeMultiplier;

        if (context.isCritical)
            rawDamage *= 1.5f;

        int finalDamage = Mathf.RoundToInt(rawDamage);
        finalDamage = Mathf.Max(1, finalDamage);

        target.TakeDamage(finalDamage);
    }
}
