using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Status/Paralysis")]
public class ParalysisStatusSO : StatusEffectSO
{
    public float skipChance = 0.25f;

    public override bool PreventAction(BattleActionType actionType)
    {
        if (actionType != BattleActionType.Move)
            return false;

        return Random.value < skipChance;
    }
}
