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

        if (ProfedexManager.Instance == null)
        {
            Debug.LogError("PROFEDEX MANAGER ES NULL");
            return;
        }

        ProfedexManager.Instance.RegisterProfessor(data);
    }
}