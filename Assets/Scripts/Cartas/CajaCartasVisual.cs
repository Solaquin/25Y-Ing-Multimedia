using System.Collections.Generic;
using UnityEngine;

public class CajaCartasVisual : MonoBehaviour
{
    public GameObject cartaPrefab;
    public Transform puntoInicio;
    public float offsetY = 0.002f;

    private List<GameObject> cartasInstanciadas = new List<GameObject>();

    public void ActualizarCartas(int cantidad)
    {
        while (cartasInstanciadas.Count < cantidad)
        {
            Vector3 pos = puntoInicio.position + Vector3.up * offsetY * cartasInstanciadas.Count;

            GameObject nueva = Instantiate(cartaPrefab, pos, puntoInicio.rotation, transform);

            cartasInstanciadas.Add(nueva);
        }
    }
}