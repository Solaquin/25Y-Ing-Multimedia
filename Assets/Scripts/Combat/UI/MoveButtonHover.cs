using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MoveButtonHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private MoveSO move;
    private MoveInfoPanel infoPanel;

    public void Setup(MoveSO move, MoveInfoPanel panel)
    {
        this.move = move;
        this.infoPanel = panel;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.Show(move);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.Hide();
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        infoPanel.Show(move);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        infoPanel.Hide();
    }
}