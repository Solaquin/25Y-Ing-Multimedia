using UnityEngine;
using UnityEngine.Events;

public class ZonaAmbiente : MonoBehaviour
{
    [Header("Eventos al entrar")]
    public UnityEvent OnEnterZona;

    [Header("Eventos al salir")]
    public UnityEvent OnExitZona;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnEnterZona.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnExitZona.Invoke();
        }
    }
}
