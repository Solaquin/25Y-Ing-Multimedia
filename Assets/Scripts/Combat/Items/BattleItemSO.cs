using UnityEngine;

[CreateAssetMenu(fileName = "BattleItemSO", menuName = "Combat/Items/BattleItemSO")]
public class BattleItemSO : ScriptableObject
{
    public string id;
    public string displayName;

    [TextArea(2, 4)]
    public string description;

    public Sprite icon;

    public BattleItemEffect effect;
    public int effectValue;

    [Tooltip("Estado que cura este item. Solo aplica si effect es HealStatusCondition.")]
    public StatusType targetStatus;

    public bool battleOnly = false;

    public bool CanUseOn(ProfemonInstance target)
    {
        switch (effect)
        {
            case BattleItemEffect.ReviveProfemon:
            case BattleItemEffect.FullRevive:
                return !target.IsAlive();

            case BattleItemEffect.HealHP:
                return target.IsAlive() && target.currentHP < target.MaxHP;

            case BattleItemEffect.HealStatusCondition:
                return target.IsAlive()
                    && target.HasStatusCondition()
                    && target.activeStatus.effect.statusType == targetStatus;

            case BattleItemEffect.FullHeal:
                // Útil si tiene vida incompleta O algún estado
                return target.IsAlive()
                    && (target.currentHP < target.MaxHP || target.HasStatusCondition());

            default:
                return target.IsAlive();
        }
    }

    public string Apply(ProfemonInstance target)
    {
        switch (effect)
        {
            case BattleItemEffect.HealHP:
                int healed = target.HealHP(effectValue);
                return $"{target.data.professorName} recuperó {healed} PS.";

            case BattleItemEffect.HealStatusCondition:
                target.CureStatusCondition();
                return $"{target.data.professorName} se curó de {targetStatus}.";

            case BattleItemEffect.FullHeal:
                target.HealHP(target.MaxHP);
                target.CureStatusCondition();
                return $"{target.data.professorName} recuperó toda su vida y se curó de sus estados.";

            case BattleItemEffect.ReviveProfemon:
                target.Revive(effectValue);
                return $"{target.data.professorName} fue revivido con {effectValue} PS.";

            case BattleItemEffect.FullRevive:
                target.Revive(target.MaxHP);
                target.CureStatusCondition();
                return $"{target.data.professorName} fue revivido con toda su vida.";

            default:
                return "No pasó nada.";
        }
    }
}