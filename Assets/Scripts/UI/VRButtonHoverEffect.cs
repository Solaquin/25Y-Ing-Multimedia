using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRButtonHoverEffect : MonoBehaviour
{
    public float hoverScale = 1.2f;
    public float speed = 8f;

    private Vector3 originalScale;
    private bool isHovering = false;

    void Start()
    {
        originalScale = transform.localScale;

        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
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
}