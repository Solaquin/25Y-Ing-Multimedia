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
        {
            Debug.LogError("BeltSlotItem missing references");
            return;
        }

        // 🔥 quitar del inventario REAL
        ItemInventory.Instance.RemoveProfeBall(myBall, 1);

        // 🔥 spawnear bola real
        belt.SpawnRealBall(
            belt.hand.position,
            belt.hand.rotation
        );

        // ❗ NO desactivamos manualmente el objeto
        // UpdateSlots lo hará automáticamente en el siguiente LateUpdate
    }
}