using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string nombreItem;
    public int precio;
    public Sprite icono;
}
