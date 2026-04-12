using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ProfeBallGrabHandler : MonoBehaviour
{
    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        //cuando la agarras, pasa a mundo físico
        ProfeBallState state = GetComponent<ProfeBallState>();

        if (state != null)
            state.SetInWorld();
    }
}