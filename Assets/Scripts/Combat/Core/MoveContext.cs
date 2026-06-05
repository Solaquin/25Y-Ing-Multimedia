using System.Collections.Generic;
using UnityEngine;

public class MoveContext
{
    public MoveSO move;
    public CombatUnit user;
    public CombatUnit target;

    public bool isCritical;

    public List<string> messages = new();

    public void AddMessage(string message)
    {
        messages.Add(message);
    }
}
