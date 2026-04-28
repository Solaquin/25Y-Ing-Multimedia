using System.Collections.Generic;
using UnityEngine;

public class CajaCartasVisual : MonoBehaviour
{
    public GameObject cartaPrefab;
    public Transform puntoInicio;
    public float offsetX = 0.002f;

    public int maxCards = 15;

    private List<GameObject> cartasInstanciadas = new List<GameObject>();

    public void ActualizarCartas(int cantidad)
    {
        if (cartasInstanciadas.Count >= maxCards)
            return;

        while (cartasInstanciadas.Count < cantidad)
        {
            Vector3 pos = puntoInicio.position + Vector3.left * offsetX * cartasInstanciadas.Count;

            GameObject nueva = Instantiate(cartaPrefab, pos, puntoInicio.rotation, transform);

            cartasInstanciadas.Add(nueva);
        }
    }
}