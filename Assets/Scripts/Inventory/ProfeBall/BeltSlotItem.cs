using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BeltSlotItem : MonoBehaviour
{
    private ProfeBallBeltSlots belt;
    private XRSimpleInteractable grab;
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
        grab = GetComponent<XRSimpleInteractable>();
        if (grab != null)
            grab.selectEntered.AddListener(OnGrab);
    }



    void OnGrab(SelectEnterEventArgs args)
    {
        if (belt == null || myBall == null) return;

        ItemInventory.Instance.ConsumeItem(myBall.id);

        var interactor = args.interactorObject;
        Transform handTransform = (interactor as MonoBehaviour)?.transform; 

        GameObject real = belt.SpawnRealBall(handTransform.position, handTransform.rotation, handTransform);
        var realGrab = real.GetComponent<XRGrabInteractable>();
        var manager = grab.interactionManager;

        if (manager != null)
        {
            // 1. Soltamos la visual
            manager.SelectExit(interactor, args.interactableObject);
            // 2. Agarramos la real
            manager.SelectEnter(interactor, realGrab);
        }

        gameObject.SetActive(false);
    }
}