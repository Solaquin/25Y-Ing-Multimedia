using UnityEngine;

public class PlayerPartyManager : MonoBehaviour
{
    public static PlayerPartyManager Instance;

    public ProfemonData starter;
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

        starter = chosen;
        starterChosen = true;

        Debug.Log("Inicial elegido: " + chosen.professorName);

        ProfedexManager.Instance.RegisterProfessor(chosen);
    }
}