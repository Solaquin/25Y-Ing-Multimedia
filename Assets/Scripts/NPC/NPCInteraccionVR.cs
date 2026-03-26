using UnityEngine;

public class NPCInteraccionVR : MonoBehaviour
{
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

        if (other.CompareTag("Player")) // 🔥 mejor que usar nombre
        {
            if (alerta != null)
            {
                alerta.SetActive(true);
                Debug.Log("El jugador entró al rango de " + gameObject.name);
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
