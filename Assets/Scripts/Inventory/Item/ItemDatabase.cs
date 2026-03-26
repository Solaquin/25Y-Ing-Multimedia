using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fuente de verdad de todos los BattleItemSO del juego.
/// ItemInventory, tiendas y cualquier otro sistema consultan aquí.
/// Solo necesitas registrar cada item una vez.
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<BattleItemSO> allItems = new List<BattleItemSO>();

    private Dictionary<string, BattleItemSO> lookup;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildLookup();
    }

    void BuildLookup()
    {
        lookup = new Dictionary<string, BattleItemSO>();
        foreach (var item in allItems)
        {
            if (item != null && !lookup.ContainsKey(item.id))
                lookup[item.id] = item;
            else if (item != null)
                Debug.LogWarning($"[ItemDatabase] ID duplicado ignorado: {item.id}");
        }
    }

    public BattleItemSO Get(string itemId)
    {
        lookup.TryGetValue(itemId, out var data);
        return data;
    }

    public List<BattleItemSO> GetAll() => new List<BattleItemSO>(allItems);
}