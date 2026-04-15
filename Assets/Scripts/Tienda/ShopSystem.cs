using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [Header("Items que vende esta tienda")]
    [Tooltip("IDs de los items disponibles. Deben existir en ItemDatabase.")]
    [SerializeField] private List<string> itemsEnVenta = new List<string>();

    [Header("UI")]
    [SerializeField] private GameObject shopItemButtonPrefab;
    [SerializeField] private Transform contenedorBotones;

    public void AbrirTienda()
    {
        GenerarBotones();
    }

    public void CerrarTienda()
    {
        LimpiarBotones();
    }

    void GenerarBotones()
    {
        LimpiarBotones();

        foreach (string itemId in itemsEnVenta)
        {
            ItemSO item = ItemDatabase.Instance.Get(itemId);

            if (item == null)
            {
                Debug.LogWarning($"[ShopSystem] Item no encontrado en ItemDatabase: {itemId}");
                continue;
            }

            GameObject buttonGO = Instantiate(shopItemButtonPrefab, contenedorBotones);
            ShopItemButton button = buttonGO.GetComponent<ShopItemButton>();

            if (button != null)
                button.Setup(item, this);
        }
    }

    void LimpiarBotones()
    {
        foreach (Transform child in contenedorBotones)
            Destroy(child.gameObject);
    }

    public void ComprarItem(ItemSO item)
    {
        if (item == null) return;

        if (!WalletSystem.Instance.Gastar(item.precio))
        {
            Debug.Log($"[Shop] Dinero insuficiente para {item.displayName}");
            return;
        }

        ItemInventory.Instance.AddItem(item.id);
        Debug.Log($"[Shop] Comprado: {item.displayName} | Saldo: {WalletSystem.Instance.Dinero}");
    }
}