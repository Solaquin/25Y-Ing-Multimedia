using UnityEngine;

public class TestCartas : MonoBehaviour
{
    public CajaCartasVisual caja;
    public InventarioCartas inventario;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventario.AgregarCarta();

            caja.ActualizarCartas(inventario.cartas);
        }
    }
}