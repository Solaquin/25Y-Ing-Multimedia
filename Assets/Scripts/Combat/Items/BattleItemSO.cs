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

    // Si es true, el item solo puede usarse durante el combate
    public bool battleOnly = false;

 
    public bool CanUseOn(ProfemonInstance target)
    {
        switch (effect)
        {
            case BattleItemEffect.ReviveProfemon:
                return !target.IsAlive();

            case BattleItemEffect.HealHP:
                return target.IsAlive() && target.currentHP < target.MaxHP;

            case BattleItemEffect.HealStatusCondition:
                return target.IsAlive() && target.HasStatusCondition();

            default:
                return target.IsAlive();
        }
    }

    /// <summary>
    /// Aplica el efecto del item sobre la instancia dada.
    /// Usar tanto en combate (via BattleItemUsageHandler)
    /// como fuera de él (via OverworldItemUsageHandler).
    /// </summary>
    public string Apply(ProfemonInstance target)
    {
        switch (effect)
        {
            case BattleItemEffect.HealHP:
                int healed = target.HealHP(effectValue);
                return $"{target.data.professorName} recuperó {healed} PS.";

            case BattleItemEffect.HealStatusCondition:
                target.CureStatusCondition();
                return $"{target.data.professorName} se curó de su estado.";

            case BattleItemEffect.ReviveProfemon:
                target.Revive(effectValue);
                return $"{target.data.professorName} fue revivido con {effectValue} PS.";

            default:
                return "No pasó nada.";
        }
    }
}