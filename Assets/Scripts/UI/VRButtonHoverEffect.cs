using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.EventSystems; // 🔥 NUEVO

public class VRButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // 🔥 NUEVO
{
    public float hoverScale = 1.2f;
    public float speed = 8f;

    private Vector3 originalScale;
    private bool isHovering = false;

    void Start()
    {
        originalScale = transform.localScale;

        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();

        // EVITA ERROR si no existe (por ejemplo en UI)
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
        }
    }

    void Update()
    {
        Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * speed
        );
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovering = true;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        isHovering = false;
    }

    // 🔥 NUEVO: soporte para UI en VR (Canvas)
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}