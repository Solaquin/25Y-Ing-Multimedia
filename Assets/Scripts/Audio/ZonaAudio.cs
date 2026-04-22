using UnityEngine;

public class ZonaAudio : MonoBehaviour
{
    public AudioInteractivo audioZona;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioZona != null)
                audioZona.ActivarAudio();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioZona != null)
                audioZona.DesactivarAudio();
        }
    }
}
