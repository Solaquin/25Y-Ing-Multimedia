using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [Header("Items Database")]
    [SerializeField]
    private ItemDatabase itemDatabase;

    [Header("UI")]
    [SerializeField] private GameObject shopItemButtonPrefab;
    [SerializeField] private Transform contenedorBotones;
    [SerializeField] private TMP_Text dineroText;

    void Start()
    {
        if (WalletSystem.Instance == null)
        {
            Debug.LogError("WalletSystem no encontrado en la escena.");
            return;
        }

        // Suscribirse al evento
        WalletSystem.Instance.OnDineroChanged += ActualizarDineroUI;

        // Mostrar valor inicial
        ActualizarDineroUI(WalletSystem.Instance.Dinero);
    }

    void OnDestroy()
    {
        if (WalletSystem.Instance != null)
        {
            WalletSystem.Instance.OnDineroChanged -= ActualizarDineroUI;
        }
    }

    void ActualizarDineroUI(int dineroActual)
    {
        if (dineroText != null)
            dineroText.text = dineroActual.ToString();
    }

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

        List<BattleItemSO> battleItems =
            itemDatabase.GetAllBattleItems();

        List<ProfeBallSO> profeBalls =
            itemDatabase.GetAllProfeBalls();

        foreach (ItemSO item in profeBalls)
        {
            CrearBoton(item);
        }

        foreach (ItemSO item in battleItems)
        {
            CrearBoton(item);
        }
    }

    void CrearBoton(ItemSO item)
    {
        if (item == null)
            return;

        GameObject buttonGO =
            Instantiate(
                shopItemButtonPrefab,
                contenedorBotones
            );

        ShopItemButton button =
            buttonGO.GetComponent<ShopItemButton>();

        if (button != null)
            button.Setup(item, this);
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