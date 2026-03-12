using UnityEngine;
using System.Collections.Generic;

public class PlayerPartyManager : MonoBehaviour
{
    public static PlayerPartyManager Instance;

    [Header("Debug Starting Party")]
    public List<PlayerPartySlot> startingParty = new List<PlayerPartySlot>();

    public List<ProfemonInstance> party = new List<ProfemonInstance>();

    public List<ProfemonInstance> storage = new List<ProfemonInstance>();

    public int maxPartySize = 6;

    public bool starterChosen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ===============================
    // DEBUG START PARTY
    // ===============================
    void Start()
    {
        if (party.Count == 0)
            InitializeStartingParty();
    }

    void InitializeStartingParty()
    {
        if (party.Count > 0)
            return;

        foreach (var slot in startingParty)
        {
            if (slot.profemon == null)
                continue;

            ProfemonInstance instance =
                new ProfemonInstance(slot.profemon, slot.level);

            party.Add(instance);
        }
    }

    // ===============================
    // STARTER
    // ===============================
    public void SetStarter(ProfemonData chosen, int level = 5)
    {
        if (starterChosen)
        {
            Debug.Log("Starter ya fue elegido.");
            return;
        }

        if (!HasSpaceInParty())
        {
            Debug.Log("No hay espacio para el starter.");
            return;
        }

        ProfemonInstance starterInstance = new ProfemonInstance(chosen, level);
        party.Add(starterInstance);

        starterChosen = true;

        Debug.Log("Starter elegido: " + chosen.professorName);

        if (ProfedexManager.Instance != null)
        {
            ProfedexManager.Instance.RegisterProfessor(chosen);
        }
    }

    // ===============================
    // MÉTODOS PARTY
    // ===============================
    public void AddToParty(ProfemonInstance instance)
    {
        if (HasSpaceInParty())
        {
            party.Add(instance);

            Debug.Log(instance.data.professorName +
                      " nivel " + instance.level +
                      " agregado a la Party.");
        }
        else
        {
            storage.Add(instance);

            Debug.Log("Party llena. Enviado al almacenamiento.");
        }

        if (ProfedexManager.Instance != null)
            ProfedexManager.Instance.RegisterProfessor(instance.data);
    }


    public bool HasSpaceInParty()
    {
        return party.Count < maxPartySize;
    }

    // ===============================
    // COMBAT INTEGRATION
    // ===============================

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

    public List<ProfemonInstance> GetAlivePartyMembers()
    {
        List<ProfemonInstance> alive = new List<ProfemonInstance>();

        foreach (var p in party)
        {
            if (p.currentHP > 0)
                alive.Add(p);
        }

        return alive;
    }

    public List<ProfemonInstance> GetParty()
    {
        return party;
    }

    public void HealParty()
    {
        foreach (var p in party)
        {
            p.currentHP = p.maxHP;
        }
    }
}