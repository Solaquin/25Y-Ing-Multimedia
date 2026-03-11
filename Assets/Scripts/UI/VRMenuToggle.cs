using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRMenuToggle : MonoBehaviour
{
    public GameObject menuRoot;
    public VRMenuManager menuManager;

    private List<InputDevice> leftHandDevices = new List<InputDevice>();

    void Start()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
    }

    void Update()
    {
        // Tecla M del teclado
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }

        // Botón de menú del mando izquierdo
        foreach (var device in leftHandDevices)
        {
            bool menuPressed;
            if (device.TryGetFeatureValue(CommonUsages.menuButton, out menuPressed) && menuPressed)
            {
                ToggleMenu();
            }
        }
    }

    void ToggleMenu()
    {
        bool isActive = menuRoot.activeSelf;

        menuRoot.SetActive(!isActive);

        if (!isActive)
        {
            menuManager.ResetMenu();
        }
    }
}