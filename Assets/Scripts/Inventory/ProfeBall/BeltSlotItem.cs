using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BeltSlotItem : MonoBehaviour
{
    private ProfeBallBeltSlots belt;
    private XRGrabInteractable grab;
    private ProfeBallSO myBall;

    public void Init(ProfeBallBeltSlots b)
    {
        belt = b;
    }

    public void SetBall(ProfeBallSO ball)
    {
        myBall = ball;
    }

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (grab != null)
            grab.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (belt == null || belt.hand == null || myBall == null)
            return;

        ItemInventory.Instance.ConsumeItem(myBall.id);

        // 🔥 obtener attach point desde la mano
        Transform attach = belt.hand;

        belt.SpawnRealBall(
            belt.hand.position,
            belt.hand.rotation,
            attach
        );
    }
}