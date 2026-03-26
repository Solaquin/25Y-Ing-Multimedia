using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public void AgregarItem(ItemData item)
    {
        Debug.Log("Item enviado al inventario: " + item.nombreItem);
    }
}
