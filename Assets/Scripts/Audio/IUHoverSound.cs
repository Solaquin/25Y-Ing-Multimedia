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
        // args.interactableObject es el objeto al que se está apuntando
        GameObject objetoHover = args.interactableObject.transform.gameObject;

        if (objetoHover == ultimoHover) return;

        ultimoHover = objetoHover;

        if (audioUI != null)
            AudioManager.Play(audioUI);
    }

    private void OnHoverSalido(HoverExitEventArgs args)
    {
        if (args.interactableObject.transform.gameObject == ultimoHover)
            ultimoHover = null;
    }
}