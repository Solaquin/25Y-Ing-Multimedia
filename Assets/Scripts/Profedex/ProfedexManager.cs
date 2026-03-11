using System.Collections.Generic;
using UnityEngine;

public class ProfedexManager : MonoBehaviour
{
    public static ProfedexManager Instance;

    public List<ProfemonData> capturedProfessors = new List<ProfemonData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterProfessor(ProfemonData data)
    {
        if (!capturedProfessors.Contains(data))
        {
            capturedProfessors.Add(data);
            Debug.Log("Registrado en la Profedex: " + data.professorName);
        }
        else
        {
            Debug.Log("Ya estaba registrado.");
        }
    }

    public void ShowProfedex()
    {
        Debug.Log("=== PROFEDEx ===");

        foreach (var prof in capturedProfessors)
        {
            Debug.Log(prof.professorName + " - " + prof.description);
        }
    }
    public bool IsRegistered(ProfemonData data)
    {
        return capturedProfessors.Contains(data);
    }
}