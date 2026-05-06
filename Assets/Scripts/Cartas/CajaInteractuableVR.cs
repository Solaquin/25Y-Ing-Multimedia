using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CajaInteractuableVR : MonoBehaviour
{
    public CartasInspectorVR inspector;

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("XR funcionando");
        inspector.ToggleInspeccion();
    }
}