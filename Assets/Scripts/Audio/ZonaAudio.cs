using UnityEngine;

public class ZonaAudio : MonoBehaviour
{
    public AudioInteractivo audioZona;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (audioZona != null)
            AudioManager.PlayLoop(audioZona);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (audioZona != null)
            AudioManager.StopLoop(audioZona);
    }
}
