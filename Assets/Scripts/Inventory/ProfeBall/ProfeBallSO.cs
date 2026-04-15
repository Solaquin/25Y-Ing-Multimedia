using UnityEngine;

/// <summary>
/// ScriptableObject para las Profeballs.
/// Extiende BattleItemSO para que el inventario y la tienda
/// las traten igual que cualquier otro item.
/// </summary>
[CreateAssetMenu(fileName = "ProfeBallSO", menuName = "Items/ProfeBall")]
public class ProfeBallSO : ItemSO
{
    [Header("Captura")]
    [Tooltip("Prefab físico que se instancia al lanzar la bola")]
    public GameObject prefabBola;

    [Tooltip("Bonus que se resta a la dificultad de captura del Profemon")]
    [Range(0, 50)]
    public int captureBonus = 0;
}
