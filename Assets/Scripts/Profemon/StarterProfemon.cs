using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StarterProfemon : MonoBehaviour
{
    public ProfemonData data;

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Guardamos posici�n y rotaci�n inicial
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.activated.AddListener(OnActivated);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
        grabInteractable.activated.RemoveListener(OnActivated);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;

        // Si NO fue elegido como inicial, vuelve a su lugar
        if (!PlayerPartyManager.Instance.starterChosen)
        {
            ResetPosition();
        }
    }

    void OnActivated(ActivateEventArgs args)
    {
        if (!isHeld) return;

        TrySelectStarter();
    }

    void TrySelectStarter()
    {
        if (PlayerPartyManager.Instance.starterChosen)
            return;

        PlayerPartyManager.Instance.SetStarter(data);

        DisableAllStarters();
        // 🧠 ACTIVA DIÁLOGO DEL NPC DESPUÉS DE ELEGIR STARTER
        NPCDialogoMovil npc = FindFirstObjectByType<NPCDialogoMovil>();

        if (npc != null)
        {
            npc.ActivarDialogoStarter();
        }
    }

    void ResetPosition()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    void DisableAllStarters()
    {
        StarterProfemon[] allStarters = FindObjectsOfType<StarterProfemon>();

        foreach (StarterProfemon starter in allStarters)
        {
            starter.gameObject.SetActive(false);
        }
    }
}