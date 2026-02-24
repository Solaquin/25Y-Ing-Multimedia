using UnityEngine;

public class Profemon : MonoBehaviour
{
    public ProfemonData data;

    public bool isCaptured = false;

    public void HideProfessor()
    {
        gameObject.SetActive(false);
    }

    public void ShowProfessor()
    {
        gameObject.SetActive(true);
    }

    public void ConfirmCapture()
    {
        Debug.Log("ConfirmCapture ejecutado");

        if (data == null)
        {
            Debug.LogError("DATA ES NULL");
            return;
        }

        if (PlayerPartyManager.Instance == null)
        {
            Debug.LogError("PLAYER PARTY MANAGER ES NULL");
            return;
        }

        if (isCaptured)
        {
            Debug.Log("Ya estaba capturado.");
            return;
        }

        // 🔥 Verificar espacio en party
        if (PlayerPartyManager.Instance.HasSpaceInParty())
        {
            PlayerPartyManager.Instance.AddToParty(data);
            Debug.Log(data.professorName + " añadido a la Party.");
        }
        else
        {
            Debug.Log("Party llena. Registrado en Profedex pero liberado.");

            if (ProfedexManager.Instance != null)
                ProfedexManager.Instance.RegisterProfessor(data);
        }

        isCaptured = true;

        // Ocultar del mundo
        HideProfessor();
    }
    }
