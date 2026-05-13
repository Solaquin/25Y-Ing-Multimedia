using System.Collections;
using UnityEngine;

/// <summary>
/// Maneja el uso de items durante el combate.
/// BattleSystem delega aquí el case BattleActionType.Item.
/// </summary>
public class BattleItemUsageHandler : MonoBehaviour
{

    /// <summary>
    /// Ejecuta el item de la acción de turno.
    /// Llamar desde BattleSystem.UseItem(action).
    /// </summary>
    public IEnumerator ExecuteItemAction(TurnAction action)
    {
        BattleItemSO item = action.item;
        ProfemonInstance target = action.itemTarget;

        if (item == null || target == null)
        {
            Debug.LogWarning("[BattleItemUsageHandler] Item o target nulo en la acción.");
            yield break;
        }

        // Verificar que el item siga en inventario (gasto defensivo)
        if (!ItemInventory.Instance.HasItem(item.id))
        {
            yield return StartCoroutine(
                BattleMessenger.Show($"¡No quedan {item.displayName}!")
            );
            yield break;
        }

        if (!item.CanUseOn(target))
        {
            yield return StartCoroutine(
                BattleMessenger.Show($"¡No se puede usar {item.displayName} ahora!")
            );
            yield break;
        }

        // Consumir y aplicar
        ItemInventory.Instance.ConsumeItem(item.id);

        string resultMessage = item.Apply(target);

        yield return StartCoroutine(BattleMessenger.Show(resultMessage));

        BattleEvents.OnHPChanged?.Invoke();

        yield return new WaitForSeconds(0.3f);
    }
}