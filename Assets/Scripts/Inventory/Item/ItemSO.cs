using UnityEngine;

/// <summary>
/// Base común para todos los items del juego.
/// BattleItemSO y ProfeBallSO heredan de aquí.
/// El inventario y la tienda trabajan con esta clase base.
/// </summary>
public abstract class ItemSO : ScriptableObject
{
    [Header("Identificación")]
    public string id;
    public string displayName;

    [TextArea(2, 4)]
    public string description;

    public Sprite icon;

    [Header("Economía")]
    public int precio;
}