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

    void OnEnable()
    {
        inventario.onCartasChanged += ActualizarCartas;
    }

    void OnDisable()
    {
        inventario.onCartasChanged -= ActualizarCartas;
    }

    public void ActualizarCartas(int cantidad)
    {
        int objetivo = Mathf.Min(cantidad, maxCards);

        while (cartasInstanciadas.Count < objetivo)
        {
            int index = cartasInstanciadas.Count;

            GameObject nueva = Instantiate(cartaPrefab, puntoInicio);
            nueva.transform.localPosition = Vector3.left * offsetX * index;
            nueva.transform.localRotation = Quaternion.identity;

            // Aplicar material del ScriptableObject
            Renderer rend = nueva.GetComponent<Renderer>()
                         ?? nueva.GetComponentInChildren<Renderer>();
            if (rend != null && index < inventario.Cartas.Count)
            {
                CartaSO datos = inventario.Cartas[index];
                if (datos.materialCarta != null)
                    rend.material = datos.materialCarta;
            }

            cartasInstanciadas.Add(nueva);
        }
    }
}