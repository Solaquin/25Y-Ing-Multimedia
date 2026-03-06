using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public TurnOrderDebugUI debugUI;
    public void ExecuteTurn(List<TurnAction> actions)
    {
        List<TurnAction> ordered =
            TurnOrderResolver.Resolve(actions);

        // Debug turn
        debugUI.ShowTurnOrder(ordered);

        foreach (var action in ordered)
        {
            if (!action.user.IsAlive())
                continue;

            UseMove(
                action.user,
                action.target,
                action.move
            );
        }
    }

    public void UseMove(CombatUnit user, CombatUnit target, MoveSO move)
    {
        if (!CheckAccuracy(user, target, move))
        {
            Debug.Log($"{move.moveName} falló");
            return;
        }

        bool isCritical = CheckCritical(user, target, move);

        MoveContext context = new MoveContext
        {
            move = move,
            user = user,
            target = target,
            isCritical = isCritical
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

    private bool CheckCritical(CombatUnit user, CombatUnit target, MoveSO move)
    {
        float userSpeed =
            user.GetStat(StatType.Speed);

        float targetSpeed =
            target.GetStat(StatType.Speed);

        if (targetSpeed <= 0)
            targetSpeed = 1;

        float critChance = move.baseCritChance * (userSpeed / targetSpeed);

        critChance = Mathf.Clamp(critChance, 0f, 100f);

        float roll = Random.Range(0f, 100f);

        Debug.Log($"Crit Roll: {roll} vs {critChance} = Crit: {roll <= critChance}");

        return roll <= critChance;
    }
}