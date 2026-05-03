using UnityEngine;

public class NPCInteraccionVR : MonoBehaviour
{
    public AudioInteractivo audioAlerta;
    [Header("Símbolo de alerta sobre el NPC")]
    public GameObject alerta;

    void Start()
    {
        if (alerta != null)
            alerta.SetActive(false);
        else
            Debug.LogWarning("No se asignó el objeto 'alerta' en el Inspector para " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter con: " + other.name);

        if (other.CompareTag("Player")) // ✅ corregido, sin espacio extra
        {
            if (alerta != null)
            {
                alerta.SetActive(true);

                if (audioAlerta != null)
                    AudioManager.Play(audioAlerta);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit con: " + other.name);

        if (other.CompareTag("Player"))
        {
            if (alerta != null)
            {
                alerta.SetActive(false);
                Debug.Log("El jugador salió del rango de " + gameObject.name);
            }
        }
    }
}
