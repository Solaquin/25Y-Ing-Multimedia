using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventario del jugador. Solo maneja cantidades.
/// Para datos del item consulta ItemDatabase.
/// </summary>
public class ItemInventory : MonoBehaviour
{
    public static ItemInventory Instance { get; private set; }

    private Dictionary<string, int> itemCounts = new Dictionary<string, int>();

#if UNITY_EDITOR
    [System.Serializable]
    public class DebugItemEntry
    {
        public BattleItemSO item;
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

    //Consulta

    public int GetCount(string itemId) =>
        itemCounts.TryGetValue(itemId, out int count) ? count : 0;

    public bool HasItem(string itemId) => GetCount(itemId) > 0;

    public BattleItemSO GetItemData(string itemId) =>
        ItemDatabase.Instance.Get(itemId);

    public List<(BattleItemSO item, int count)> GetAllItems()
    {
        var result = new List<(BattleItemSO, int)>();
        foreach (var kv in itemCounts)
        {
            if (kv.Value <= 0) continue;
            var data = ItemDatabase.Instance.Get(kv.Key);
            if (data != null) result.Add((data, kv.Value));
        }
        return result;
    }

    //Modificacion

    public void AddItem(string itemId, int amount = 1)
    {
        if (!itemCounts.ContainsKey(itemId))
            itemCounts[itemId] = 0;

        itemCounts[itemId] += amount;
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
        Debug.Log($"[Inventory] -1 {itemId} - quedan: {itemCounts[itemId]}");
        return true;
    }

    //Debug

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

        if (items.Count == 0)
        {
            Debug.Log("[Inventario] Vacío.");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"[Inventario] {items.Count} tipo(s):");

        foreach (var (item, count) in items)
            sb.AppendLine($"  • {item.displayName} x{count}  (id: {item.id})");

        Debug.Log(sb.ToString());
    }

    [ContextMenu("Debug/Consumir 1 de cada item")]
    void Debug_ConsumeOne()
    {
        foreach (var (item, _) in GetAllItems())
            ConsumeItem(item.id);

        Debug_PrintInventory();
    }
#endif
}