using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ItemButtonHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private ItemSO item;
    private ItemInfoPanel infoPanel;

    public void Setup(ItemSO item, ItemInfoPanel panel)
    {
        this.item = item;
        this.infoPanel = panel;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.Show(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.Hide();
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        infoPanel.Show(item);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        infoPanel.Hide();
    }
}