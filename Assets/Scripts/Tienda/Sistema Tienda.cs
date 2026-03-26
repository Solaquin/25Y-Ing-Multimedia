using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public int dineroJugador = 100;

    public void ComprarItem(ItemData item)
    {
        if (item == null)
        {
            Debug.Log("Item no valido");
            return;
        }

        if (dineroJugador >= item.precio)
        {
            dineroJugador -= item.precio;

            Debug.Log("Compraste: " + item.nombreItem);
            Debug.Log("Dinero restante: " + dineroJugador);

            // Aquí luego se enviará al inventario
        }
        else
        {
            Debug.Log("No tienes suficiente dinero");
        }
    }
}
