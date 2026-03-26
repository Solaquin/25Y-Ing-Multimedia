using System.Collections.Generic;
using UnityEngine;

public class NPCParty : MonoBehaviour
{
    public List<ProfemonInstance> party = new List<ProfemonInstance>();

    public ProfemonInstance GetFirstAlive()
    {
        foreach (var p in party)
        {
            if (p.currentHP > 0)
                return p;
        }

        return null;
    }

    public bool HasAvailable()
    {
        foreach (var p in party)
        {
            if (p.currentHP > 0)
                return true;
        }

        return false;
    }
}