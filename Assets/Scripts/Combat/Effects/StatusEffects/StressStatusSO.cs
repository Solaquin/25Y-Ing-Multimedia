using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Status/Stress")]
public class StressStatusSO : StatusEffectSO
{
    public float damagePercent = 0.1f;

    public override void OnTurnEnd(CombatUnit unit)
    {
        int damage =
            Mathf.RoundToInt(unit.GetMaxHP() * damagePercent);

        unit.TakeDamage(damage);

        Debug.Log($"{unit.name} sufrió daño por estrés");
    }
}
