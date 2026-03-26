using System.Collections.Generic;
using UnityEngine;

public class NPCParty : MonoBehaviour
{
    public List<NPCPartySlot> partySlots = new List<NPCPartySlot>();

    public List<ProfemonInstance> party = new List<ProfemonInstance>();
    void Awake()
    {
        GenerateParty();
    }

    void GenerateParty()
    {
        party.Clear();

        foreach (var slot in partySlots)
        {
            ProfemonInstance instance =
                new ProfemonInstance(slot.profemon, slot.level);

            party.Add(instance);
        }
    }

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
    public ProfemonInstance GetNextAlive(ProfemonInstance current)
    {
        foreach (var p in party)
        {
            if (p != current && p.currentHP > 0)
                return p;
        }

        return null;
    }

    public ProfemonInstance GetRandomAlive()
    {
        List<ProfemonInstance> alive = new List<ProfemonInstance>();

        foreach (var p in party)
        {
            if (p.currentHP > 0)
                alive.Add(p);
        }

        if (alive.Count == 0)
            return null;

        return alive[Random.Range(0, alive.Count)];
    }
}