using UnityEngine;
using System.Collections.Generic;

public class PlayerPartyManager : MonoBehaviour
{
    public static PlayerPartyManager Instance;

    public List<ProfemonInstance> party = new List<ProfemonInstance>();

    public int maxPartySize = 6;

    public bool starterChosen = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetStarter(ProfemonData chosen)
    {
        if (starterChosen) return;

        AddToParty(chosen);
        starterChosen = true;

        Debug.Log("Starter elegido: " + chosen.professorName);
    }

    public void AddToParty(ProfemonData data)
    {
        if (party.Count >= maxPartySize)
        {
            Debug.Log("Party llena.");
            return;
        }

        ProfemonInstance newProfemon = new ProfemonInstance(data);
        party.Add(newProfemon);

        Debug.Log(data.professorName + " agregado a la Party.");
        Debug.Log("Cantidad en party: " + PlayerPartyManager.Instance.party.Count);

        // 🔥 Registrar automáticamente en la Profedex
        if (ProfedexManager.Instance != null)
        {
            ProfedexManager.Instance.RegisterProfessor(data);
        }
    }
    public bool HasSpaceInParty()
    {
        return party.Count < maxPartySize;
    }
}