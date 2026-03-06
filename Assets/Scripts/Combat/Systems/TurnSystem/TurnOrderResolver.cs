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
            .OrderByDescending(a => a.priority)
            .ThenByDescending(a => a.speed)
            .ThenByDescending(a => Random.value)
            .ToList();
    }
}