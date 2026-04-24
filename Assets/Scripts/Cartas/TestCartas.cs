using UnityEngine;

public class TestCartas : MonoBehaviour
{
    public CajaCartasVisual caja;

    private int cartas = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cartas++;
            caja.ActualizarCartas(cartas);
        }
    }
}