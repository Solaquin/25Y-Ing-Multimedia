using System.Collections;
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

        GameObject real = belt.SpawnRealBall(handTransform.position, handTransform.rotation);
        var realGrab = real.GetComponent<XRGrabInteractable>();
        var manager = grab.interactionManager;

        if (manager != null && realGrab != null)
        {
            manager.SelectExit(interactor, args.interactableObject);
            StartCoroutine(DelayedGrab(manager, interactor, realGrab));
            return;
        }

        gameObject.SetActive(false);
    }

    IEnumerator DelayedGrab(
        UnityEngine.XR.Interaction.Toolkit.XRInteractionManager manager,
        UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor,
        XRGrabInteractable realGrab)
    {
        yield return null;
        manager.SelectEnter(interactor, realGrab);
        gameObject.SetActive(false);
    }
}