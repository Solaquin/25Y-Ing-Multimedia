using UnityEngine;

/// <summary>
/// Maneja el uso de items fuera del combate (menú de mochila, overworld).
/// </summary>
public class OverworldItemUsageHandler : MonoBehaviour
{
    public static OverworldItemUsageHandler Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Intenta usar el item sobre la instancia dada fuera del combate.
    /// Devuelve el mensaje de resultado, o null si no se pudo usar.
    /// </summary>
    public string TryUseItem(BattleItemSO item, ProfemonInstance target)
    {
        if (item == null || target == null)
            return null;

        if (item.battleOnly)
        {
            return $"¡{item.displayName} solo puede usarse en combate!";
        }

        if (!ItemInventory.Instance.HasItem(item.id))
        {
            return $"No quedan {item.displayName}.";
        }

        if (!item.CanUseOn(target))
        {
            return $"No se puede usar {item.displayName} en {target.data.professorName} ahora.";
        }

        ItemInventory.Instance.ConsumeItem(item.id);

        string resultMessage = item.Apply(target);

        Debug.Log($"[Overworld] Item usado: {resultMessage}");

        return resultMessage;
    }
}