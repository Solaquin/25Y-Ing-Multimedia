using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fuente de verdad de todos los items del juego.
/// Es un ScriptableObject — vive en Resources/ItemDatabase.
/// No depende de ninguna escena.
/// 
/// Setup:
/// 1. Crear el asset desde Items/ItemDatabase en el menú Create
/// 2. Moverlo a la carpeta Resources/ (crearla si no existe)
/// 3. Registrar todos los BattleItemSO y ProfeBallSO en las listas del Inspector
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    private static ItemDatabase _instance;
    [SerializeField] private int totalItems;
    public int TotalItems => totalItems;

    public static ItemDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ItemDatabase>("ItemDatabase");

                if (_instance == null)
                    Debug.LogError("[ItemDatabase] No se encontró 'ItemDatabase' en la carpeta Resources/");
                else
                    _instance.BuildLookup();
            }

            return _instance;
        }
    }

    [SerializeField] private List<BattleItemSO> battleItems = new List<BattleItemSO>();
    [SerializeField] private List<ProfeBallSO> profeBalls = new List<ProfeBallSO>();

    private Dictionary<string, ItemSO> lookup;

    void BuildLookup()
    {
        lookup = new Dictionary<string, ItemSO>();
        RegisterList(battleItems);
        RegisterList(profeBalls);

        totalItems = lookup.Count;
    }

    void RegisterList<T>(List<T> items) where T : ItemSO
    {
        foreach (var item in items)
        {
            if (item == null) continue;

            if (lookup.ContainsKey(item.id))
            {
                Debug.LogWarning($"[ItemDatabase] ID duplicado ignorado: {item.id}");
                continue;
            }

            lookup[item.id] = item;
        }
    }

    /// <summary>
    /// Devuelve cualquier item por id, sin importar su tipo.
    /// </summary>
    public ItemSO Get(string itemId)
    {
        if (lookup == null) BuildLookup();
        lookup.TryGetValue(itemId, out var item);
        return item;
    }

    /// <summary>
    /// Devuelve el item casteado al tipo esperado, o null si no es ese tipo.
    /// Ejemplo: ItemDatabase.Instance.Get<ProfeBallSO>("ball_normal")
    /// </summary>
    public T Get<T>(string itemId) where T : ItemSO
    {
        return Get(itemId) as T;
    }

    public List<BattleItemSO> GetAllBattleItems() => new List<BattleItemSO>(battleItems);
    public List<ProfeBallSO> GetAllProfeBalls() => new List<ProfeBallSO>(profeBalls);

#if UNITY_EDITOR
    [ContextMenu("Rebuild Lookup")]
    void Editor_RebuildLookup() => BuildLookup();
#endif
}