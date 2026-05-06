using UnityEngine;
using System.Collections;

public class ProfesorController : MonoBehaviour
{
    [Header("Aparición")]
    public float alturaOffset = -1f;
    public float tiempoAparicion = 1f;

    [Header("Interacción")]
    public Collider zonaInteraccion; // trigger del profesor o área cercana

    private Vector3 posicionFinal;

    void Awake()
    {
        posicionFinal = transform.position;

        // empieza oculto
        transform.position = posicionFinal + new Vector3(0, alturaOffset, 0);
        gameObject.SetActive(false);

        if (zonaInteraccion != null)
            zonaInteraccion.enabled = false;
    }

    public void Aparecer()
    {
        gameObject.SetActive(true);

        if (zonaInteraccion != null)
            zonaInteraccion.enabled = true;

        StartCoroutine(AnimarAparicion());
    }

    IEnumerator AnimarAparicion()
    {
        Vector3 inicio = transform.position;
        float t = 0f;

        while (t < tiempoAparicion)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, posicionFinal, t / tiempoAparicion);
            yield return null;
        }

        transform.position = posicionFinal;
    }
}   