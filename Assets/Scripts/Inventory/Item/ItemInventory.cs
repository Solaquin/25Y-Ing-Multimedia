using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventario del jugador. Solo maneja cantidades por id.
/// Para datos del item consulta ItemDatabase.
/// Funciona igual para BattleItemSO y ProfeBallSO.
/// </summary>
public class ItemInventory : MonoBehaviour
{
    public static ItemInventory Instance { get; private set; }

    private Dictionary<string, int> itemCounts = new Dictionary<string, int>();

    /// <summary>
    /// Se dispara cada vez que cambia la cantidad de un item. Pasa el id del item modificado.
    /// El cinturón y cualquier UI se suscriben aquí para actualizar sus contadores.
    /// </summary>
    public event Action<string> OnInventoryChanged;

#if UNITY_EDITOR
    [System.Serializable]
    public class DebugItemEntry
    {
        public ItemSO item;
        public int amount = 1;
    }

    [Header("Debug")]
    [SerializeField] private List<DebugItemEntry> debugItems;
#endif

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Consulta

    public int GetCount(string itemId) =>
        itemCounts.TryGetValue(itemId, out int count) ? count : 0;

    public bool HasItem(string itemId) => GetCount(itemId) > 0;

    public ItemSO GetItemData(string itemId) =>
        ItemDatabase.Instance.Get(itemId);

    /// <summary>
    /// Devuelve todos los items con cantidad mayor a 0.
    /// </summary>
    public List<(ItemSO item, int count)> GetAllItems()
    {
        var result = new List<(ItemSO, int)>();
        foreach (var kv in itemCounts)
        {
            if (kv.Value <= 0) continue;
            var data = ItemDatabase.Instance.Get(kv.Key);
            if (data != null) result.Add((data, kv.Value));
        }
        return result;
    }

    /// <summary>
    /// Devuelve solo los BattleItems con cantidad mayor a 0.
    /// </summary>
    public List<(BattleItemSO item, int count)> GetBattleItems()
    {
        var result = new List<(BattleItemSO, int)>();
        foreach (var kv in itemCounts)
        {
            if (kv.Value <= 0) continue;
            var data = ItemDatabase.Instance.Get<BattleItemSO>(kv.Key);
            if (data != null) result.Add((data, kv.Value));
        }
        return result;
    }

    /// <summary>
    /// Devuelve solo las ProfeBalls con cantidad mayor a 0.
    /// </summary>
    public List<(ProfeBallSO item, int count)> GetProfeBalls()
    {
        var result = new List<(ProfeBallSO, int)>();
        foreach (var kv in itemCounts)
        {
            if (kv.Value <= 0) continue;
            var data = ItemDatabase.Instance.Get<ProfeBallSO>(kv.Key);
            if (data != null) result.Add((data, kv.Value));
        }
        return result;
    }

    // Modificación

    public void AddItem(string itemId, int amount = 1)
    {
        if (!itemCounts.ContainsKey(itemId))
            itemCounts[itemId] = 0;

        itemCounts[itemId] += amount;
        OnInventoryChanged?.Invoke(itemId);
        Debug.Log($"[Inventory] +{amount} {itemId} - total: {itemCounts[itemId]}");
    }

    public bool ConsumeItem(string itemId)
    {
        if (!HasItem(itemId))
        {
            Debug.LogWarning($"[Inventory] Sin stock: {itemId}");
            return false;
        }

        itemCounts[itemId]--;
        OnInventoryChanged?.Invoke(itemId);
        Debug.Log($"[Inventory] -1 {itemId} - quedan: {itemCounts[itemId]}");
        return true;
    }

    /// <summary>
    /// Usado exclusivamente por el SaveManager al cargar una partida.
    /// Reemplaza todo el inventario con los datos guardados.
    /// </summary>
    public void CargarInventario(Dictionary<string, int> datos)
    {
        itemCounts = new Dictionary<string, int>(datos);
    }

    public Dictionary<string, int> GetRawCounts() =>
        new Dictionary<string, int>(itemCounts);

    // Debug

#if UNITY_EDITOR
    [ContextMenu("Debug/Cargar debug items")]
    void Debug_LoadItems()
    {
        if (debugItems == null) return;
        foreach (var entry in debugItems)
        {
            if (entry.item == null || entry.amount <= 0) continue;
            AddItem(entry.item.id, entry.amount);
        }
    }

    [ContextMenu("Debug/Imprimir inventario")]
    void Debug_PrintInventory()
    {
        var items = GetAllItems();
        if (items.Count == 0) { Debug.Log("[Inventario] Vacío."); return; }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"[Inventario] {items.Count} tipo(s):");
        foreach (var (item, count) in items)
            sb.AppendLine($"  • {item.displayName} x{count}  (id: {item.id})");

        Debug.Log(sb.ToString());
    }
    public void RemoveProfeBall(ProfeBallSO ball, int amount)
    {
        if (ball == null) return;

        string id = ball.id;

        if (!itemCounts.ContainsKey(id))
            return;

        itemCounts[id] -= amount;

        if (itemCounts[id] <= 0)
            itemCounts.Remove(id);

        OnInventoryChanged?.Invoke(id);

        Debug.Log($"[Inventory] -{amount} {id} (ProfeBall) restante: {GetCount(id)}");
    }
#endif
}