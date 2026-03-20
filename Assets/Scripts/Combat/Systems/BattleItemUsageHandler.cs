using System.Collections;
using UnityEngine;

public class BattleItemUsageHandler : MonoBehaviour
{
    [SerializeField] private BattleTextBox textBox;
    [SerializeField] private BattleSystem battleSystem;

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
                textBox.ShowMessage($"¡No quedan {item.displayName}!")
            );
            yield break;
        }

        if (!item.CanUseOn(target))
        {
            yield return StartCoroutine(
                textBox.ShowMessage($"¡No se puede usar {item.displayName} ahora!")
            );
            yield break;
        }

        // Consumir y aplicar
        ItemInventory.Instance.ConsumeItem(item.id);

        string resultMessage = item.Apply(target);

        yield return StartCoroutine(textBox.ShowMessage(resultMessage));

        BattleEvents.OnActiveUnitChanged?.Invoke();

        yield return new WaitForSeconds(0.3f);
    }
}