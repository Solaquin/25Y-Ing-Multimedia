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

        // Consumimos del inventario
        ItemInventory.Instance.ConsumeItem(myBall.id);

        // Spawneamos la real exactamente como antes
        Transform attach = belt.hand;
        //GameObject real = belt.SpawnRealBall(belt.hand.position,belt.hand.rotation,attach);
        GameObject real = belt.SpawnRealBall(transform.position, transform.rotation, attach);

        var realGrab = real.GetComponent<XRGrabInteractable>();

        // 🔥 TRANSFERENCIA DEL GRAB: la mano ahora agarra la real, no la visual
        var interactor = args.interactorObject; // la mano/controlador
        if (interactor != null && realGrab != null)
        {
            var manager = grab.interactionManager; // usamos el manager de la visual (ya está registrado)

            if (manager != null)
            {
                // 1. Soltamos la visual
                manager.SelectExit(interactor, args.interactableObject);
                // 2. Agarramos la real
                manager.SelectEnter(interactor, realGrab);
            }
        }

        // Ocultamos la visual inmediatamente (para que no se vea ni se pueda volver a agarrar)
        gameObject.SetActive(false);
    }
}