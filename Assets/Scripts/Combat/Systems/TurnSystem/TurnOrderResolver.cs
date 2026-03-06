using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TurnOrderResolver
{
    public static List<TurnAction> Resolve(
        List<TurnAction> actions
    )
    {
        Debug.Log("Resolving turn order...");

        foreach (var action in actions)
        {
            Debug.Log(action);
        }

        return actions
            .OrderByDescending(a => a.move.priority)
            .ThenByDescending(a => a.user.GetStat(StatType.Speed))
            .ThenByDescending(a => Random.value)
            .ToList();
    }
}