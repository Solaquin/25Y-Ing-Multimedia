using UnityEngine;

public class InventarioCartas : MonoBehaviour
{
    public int cartas = 0;

    public delegate void OnCartasChanged(int cantidad);
    public event OnCartasChanged onCartasChanged;

    public void AgregarCarta()
    {
        cartas++;
        onCartasChanged?.Invoke(cartas);
    }
}