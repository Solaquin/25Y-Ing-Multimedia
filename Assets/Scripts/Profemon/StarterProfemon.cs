using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StarterProfemon : MonoBehaviour
{
    public ProfemonData data;

    [Header("Face Player Settings")]
    public float rotationSpeed = 10f;

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Transform playerCamera;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (Camera.main != null)
            playerCamera = Camera.main.transform;
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

    void Update()
    {
        if (!isHeld || playerCamera == null) return;

        Vector3 directionToPlayer = playerCamera.position - transform.position;
        if (directionToPlayer.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
        grabInteractable.trackRotation = false;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        grabInteractable.trackRotation = true;

        if (!PlayerPartyManager.Instance.starterChosen)
            ResetPosition();
    }

    void OnActivated(ActivateEventArgs args)
    {
        if (!isHeld) return;
        TrySelectStarter();
    }

    void TrySelectStarter()
    {
        if (PlayerPartyManager.Instance.starterChosen) return;

        PlayerPartyManager.Instance.SetStarter(data);
        DisableAllStarters();

        NPCDialogoMovil npc = FindFirstObjectByType<NPCDialogoMovil>();
        if (npc != null)
            npc.ActivarDialogoStarter();
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
            starter.gameObject.SetActive(false);
    }
}