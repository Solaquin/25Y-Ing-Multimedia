using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Socket del cinturón que representa un slot de Profeball.
/// Cuando el jugador interactúa con él, instancia la bola en su mano
/// y consume una unidad del ItemInventory.
/// </summary>
public class BeltSocketSlot : MonoBehaviour
{
    [Tooltip("SO de la Profeball que representa este slot")]
    public ProfeBallSO profeBallData;

    [SerializeField] private TextMeshProUGUI quantityText;

    private XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(OnSelectEntered);

        // Suscribirse a cambios del inventario para actualizar el contador
        if (ItemInventory.Instance != null)
            ItemInventory.Instance.OnInventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);

        if (ItemInventory.Instance != null)
            ItemInventory.Instance.OnInventoryChanged -= HandleInventoryChanged;
    }

    private void Start()
    {
        UpdateVisual();
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        TryGiveBall(args.interactorObject);
    }

    void TryGiveBall(IXRSelectInteractor interactor)
    {
        if (profeBallData == null)
        {
            Debug.LogWarning("[BeltSocketSlot] No hay ProfeBallSO asignado.");
            return;
        }

        if (!ItemInventory.Instance.HasItem(profeBallData.id))
        {
            Debug.Log("[BeltSocketSlot] Sin stock de " + profeBallData.displayName);
            return;
        }

        SpawnBall(interactor);
    }

    void SpawnBall(IXRSelectInteractor interactor)
    {
        if (profeBallData.prefabBola == null)
        {
            Debug.LogError("[BeltSocketSlot] ProfeBallSO no tiene prefabBola asignado.");
            return;
        }

        Transform attach = interactor.GetAttachTransform(null);

        GameObject ball = Instantiate(
            profeBallData.prefabBola,
            attach.position,
            attach.rotation
        );

        // Asignar el SO a la bola instanciada
        var pb = ball.GetComponent<Profebola>();
        if (pb != null)
            pb.datos = profeBallData;

        // Forzar que la mano la agarre automáticamente
        var interactable = ball.GetComponent<XRGrabInteractable>();
        if (interactable != null)
            interactable.interactionManager.SelectEnter(interactor, interactable);
    }

    void UpdateVisual()
    {
        if (quantityText == null || profeBallData == null) return;

        int amount = ItemInventory.Instance.GetCount(profeBallData.id);
        quantityText.text = amount.ToString();
    }

    void HandleInventoryChanged(string itemId)
    {
        if (profeBallData != null && itemId == profeBallData.id)
            UpdateVisual();
    }
}