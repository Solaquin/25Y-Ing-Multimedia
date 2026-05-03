using System.Collections.Generic;
using UnityEngine;

public class InventarioCartas : MonoBehaviour
{
    private List<CartaSO> cartasDesbloqueadas = new List<CartaSO>();

    public int cartas => cartasDesbloqueadas.Count;
    public List<CartaSO> Cartas => cartasDesbloqueadas;

    public delegate void OnCartasChanged(int cantidad);
    public event OnCartasChanged onCartasChanged;

    public void AgregarCarta(CartaSO carta)
    {
        if (carta == null) return;
        if (cartasDesbloqueadas.Contains(carta))
        {
            Debug.Log($"[Inventario] Ya tienes: {carta.nombreCarta}");
            return;
        }
        cartasDesbloqueadas.Add(carta);
        Debug.Log($"[Inventario] Nueva carta: {carta.nombreCarta} (total: {cartas})");
        onCartasChanged?.Invoke(cartas);
    }

    public bool TieneCarta(CartaSO carta) => cartasDesbloqueadas.Contains(carta);
}