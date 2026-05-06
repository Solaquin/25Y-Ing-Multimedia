using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class VRMenuToggle : MonoBehaviour
{
    public GameObject menuRoot;
    public VRMenuManager menuManager;

    [Header("Input VR")]
    public InputActionReference botonMenu;

    private PartyMenuManager partyMenuManager;

    private void Start()
    {
        if(botonMenu != null)
            botonMenu.action.Enable();

        partyMenuManager = FindFirstObjectByType<PartyMenuManager>();
    }

    void Update()
    {
        // Tecla M del teclado
        if (Input.GetKeyDown(KeyCode.M) || botonMenu.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        bool isActive = menuRoot.activeSelf;
        partyMenuManager.RefreshParty();

        menuRoot.SetActive(!isActive);

        if (!isActive)
        {
            menuManager.ResetMenu();
        }
    }
}