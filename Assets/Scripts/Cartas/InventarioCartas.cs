using System.Collections.Generic;
using UnityEngine;

public class InventarioCartas : MonoBehaviour
{
    public int cartas = 0;

    public List<Color> colores = new List<Color>();

    public delegate void OnCartasChanged(int cantidad);
    public event OnCartasChanged onCartasChanged;

    public void AgregarCarta()
    {
        cartas++;

        // 🔥 Generar color bonito
        Color nuevoColor = Color.HSVToRGB(Random.value, 0.6f, 1f);
        colores.Add(nuevoColor);

        onCartasChanged?.Invoke(cartas);
    }
}