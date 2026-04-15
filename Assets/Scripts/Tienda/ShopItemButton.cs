using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente del prefab de botón de tienda.
/// ShopSystem lo configura con el item correspondiente al generarlo.
/// </summary>
public class ShopItemButton : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image iconoItem;
    [SerializeField] private TextMeshProUGUI nombreText;
    [SerializeField] private TextMeshProUGUI precioText;
    [SerializeField] private Button botonComprar;

    private ItemSO itemData;
    private ShopSystem shopSystem;


    /// <summary>
    /// Llamado por ShopSystem al instanciar el botón.
    /// </summary>
    public void Setup(ItemSO item, ShopSystem shop)
    {
        itemData = item;
        shopSystem = shop;

        if (iconoItem != null) iconoItem.sprite = item.icon;
        if (nombreText != null) nombreText.text = item.displayName;
        if (precioText != null) precioText.text = $"${item.precio}";

        botonComprar = GetComponent<Button>();

        botonComprar.onClick.AddListener(OnComprarPressed);
    }

    void OnComprarPressed()
    {
        shopSystem.ComprarItem(itemData);
    }

    private void OnDestroy()
    {
        botonComprar.onClick.RemoveListener(OnComprarPressed);
    }
}