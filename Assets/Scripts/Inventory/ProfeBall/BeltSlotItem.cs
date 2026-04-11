using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class BeltSlotItem : MonoBehaviour
{
    private ProfeBallBeltSlots belt;
    private XRGrabInteractable grab;

    public void Init(ProfeBallBeltSlots b)
    {
        belt = b;
    }

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (grab != null)
            grab.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (belt == null)
        {
            Debug.LogError("BELT ES NULL en BeltSlotItem");
            return;
        }

        if (belt.hand == null)
        {
            Debug.LogError("HAND ES NULL en BeltSlotItem");
            return;
        }

        belt.SpawnRealBall(
            belt.hand.position,
            belt.hand.rotation
        );

        gameObject.SetActive(false);
    }
}