using System.Collections.Generic;
using UnityEngine;

public class CajaCartasVisual : MonoBehaviour
{
    public GameObject cartaPrefab;
    public Transform puntoInicio;
    public float offsetX = 0.002f;

    public int maxCards = 15;

    public InventarioCartas inventario;

    private List<GameObject> cartasInstanciadas = new List<GameObject>();

    public void ActualizarCartas(int cantidad)
    {
        int objetivo = Mathf.Min(cantidad, maxCards);

        while (cartasInstanciadas.Count < objetivo)
        {
            int index = cartasInstanciadas.Count;

            GameObject nueva = Instantiate(cartaPrefab, puntoInicio);

            // 🔥 POSICIÓN LOCAL (SOLUCIÓN AL DRIFT)
            nueva.transform.localPosition = Vector3.left * offsetX * index;
            nueva.transform.localRotation = Quaternion.identity;

            // 🔥 COLOR
            Renderer rend = nueva.GetComponentInChildren<Renderer>();
            if (rend != null && index < inventario.colores.Count)
            {
                rend.material = new Material(rend.material);
                rend.material.color = inventario.colores[index];
            }

            cartasInstanciadas.Add(nueva);
        }
    }
}