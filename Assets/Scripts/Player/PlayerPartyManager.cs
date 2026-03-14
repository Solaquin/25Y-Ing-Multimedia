using UnityEngine;
using System.Collections.Generic;

public class PlayerPartyManager : MonoBehaviour
{
    public static PlayerPartyManager Instance;

    public List<ProfemonInstance> party = new List<ProfemonInstance>();

    public int maxPartySize = 6;

    public List<ProfemonInstance> storage = new List<ProfemonInstance>();

    public int maxStorageSize = 100;

    public bool starterChosen = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
    // MÉTODO NUEVO (RECOMENDADO)
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
            AddToStorage(instance);
        }

        if (ProfedexManager.Instance != null)
        {
            ProfedexManager.Instance.RegisterProfessor(instance.data);
        }
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
    public void AddToStorage(ProfemonInstance instance)
    {
        if (storage.Count >= maxStorageSize)
        {
            Debug.Log("Storage lleno.");
            return;
        }

        storage.Add(instance);

        Debug.Log(instance.data.professorName +
                  " fue enviado al Storage.");
    }
}