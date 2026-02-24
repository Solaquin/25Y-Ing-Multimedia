using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public void UseMove(CombatUnit user, CombatUnit target,MoveSO move)
    {
        if (!CheckAccuracy(user, target, move))
        {
            Debug.Log($"{move.moveName} falló");
            return;
        }

        MoveContext context = new MoveContext
        {
            move = move,
            user = user,
            target = target
        };

        move.effect.Execute(user, target, context);
    }

    private bool CheckAccuracy(CombatUnit user, CombatUnit target, MoveSO move)
    {
        float moveAccuracy = move.accuracy;

        print($"{move} +: Accuracy: {moveAccuracy}");

        float userAccuracy = user.GetStat(StatType.Accuracy) / 100.0f;

        float targetEvasion = target.GetStat(StatType.Evasion) / 100.0f;

        if (targetEvasion <= 0)
            targetEvasion = 1.0f;

        float finalAccuracy = moveAccuracy * (userAccuracy / targetEvasion);

        finalAccuracy = Mathf.Clamp(finalAccuracy, 0f, 100f);

        int roll = Random.Range(0, 101);

        Debug.Log($"Accuracy Roll: {roll} vs {finalAccuracy}");

        return roll <= finalAccuracy;
    }
}