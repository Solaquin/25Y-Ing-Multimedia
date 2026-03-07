using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Status/BurnOut")]
public class BurnOutSO : StatusEffectSO
{
    public float damagePercent = 0.05f;

    public override void OnTurnEnd(CombatUnit unit)
    {
        int damage =
            Mathf.RoundToInt(unit.GetMaxHP() * damagePercent);

        unit.TakeDamage(damage);

        Debug.Log($"{unit.name} sufrió daño por quemadura");
    }
}
