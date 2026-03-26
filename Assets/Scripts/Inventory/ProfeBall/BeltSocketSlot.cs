using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BeltSocketSlot : MonoBehaviour
{
    public BallType ballType;
    public GameObject ballPrefab;

    [SerializeField] private TextMesh quantityText;

    private XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        TryGiveBall(args.interactorObject);
    }

    void TryGiveBall(IXRSelectInteractor interactor)
    {
        var inventory = ProfeBallInventoryManager.Instance.Inventory;

        if (!inventory.Use(ballType))
        {
            Debug.Log("Sin bolas");
            return;
        }

        SpawnBall(interactor);
    }

    void SpawnBall(IXRSelectInteractor interactor)
    {
        Transform attach = interactor.GetAttachTransform(null);

        GameObject ball = Instantiate(ballPrefab, attach.position, attach.rotation);

        var interactable = ball.GetComponent<XRGrabInteractable>();
        var pb = ball.GetComponent<Profebola>();

        pb.ballType = ballType;

        // Forzar que la mano la agarre automáticamente
        interactable.interactionManager.SelectEnter(interactor, interactable);
    }
    void UpdateVisual()
    {
        int amount = ProfeBallInventoryManager.Instance.Inventory.GetBall(ballType);
        quantityText.text = amount.ToString();
    }
}