using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioInteractivo audioUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioUI != null)
            audioUI.ActivarAudio();
    }
}
