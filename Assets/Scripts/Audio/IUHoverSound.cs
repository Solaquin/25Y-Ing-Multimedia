using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class UIHoverGlobal : MonoBehaviour
{
    public AudioInteractivo audioUI;

    private GameObject ultimoHover;

    public NearFarInteractor nfInteractorLeft;
    public NearFarInteractor nfInteractorRight;

    // 🔧 CONTROL ANTI-SPAM (CLAVE PARA VR)
    private float hoverCooldown = 0.2f;
    private float lastHoverTime = -1f;

    void OnEnable()
    {
        RegistrarEventos(nfInteractorLeft);
        RegistrarEventos(nfInteractorRight);
    }

    void OnDisable()
    {
        DesregistrarEventos(nfInteractorLeft);
        DesregistrarEventos(nfInteractorRight);
    }

    private void RegistrarEventos(NearFarInteractor interactor)
    {
        if (interactor == null) return;

        interactor.hoverEntered.AddListener(OnHoverEntrado);
        interactor.hoverExited.AddListener(OnHoverSalido);
    }

    private void DesregistrarEventos(NearFarInteractor interactor)
    {
        if (interactor == null) return;

        interactor.hoverEntered.RemoveListener(OnHoverEntrado);
        interactor.hoverExited.RemoveListener(OnHoverSalido);
    }

    private void OnHoverEntrado(HoverEnterEventArgs args)
    {
        if (args.interactableObject == null) return;

        GameObject objetoHover = args.interactableObject.transform.gameObject;

        // 🚫 Evita repetir sobre el mismo objeto
        if (objetoHover == ultimoHover) return;

        // 🚫 Evita spam por múltiples eventos en VR
        if (Time.time - lastHoverTime < hoverCooldown) return;

        ultimoHover = objetoHover;
        lastHoverTime = Time.time;

        // 🔊 Sonido hover
        if (audioUI != null)
            AudioManager.Play(audioUI);
    }

    private void OnHoverSalido(HoverExitEventArgs args)
    {
        if (args.interactableObject == null) return;

        if (args.interactableObject.transform.gameObject == ultimoHover)
        {
            ultimoHover = null;
        }
    }
}
