using UnityEngine;

public class VRMenuManager : MonoBehaviour
{
    [Header("Menu Root")]
    public GameObject menuRoot;

    [Header("Canvas")]
    public GameObject menuPrincipal;
    public GameObject profedexCanvas;
    public GameObject inventarioCanvas;
    public GameObject mapaCanvas;
    public GameObject ajustesCanvas;

    void Start()
    {
        ResetMenu();
    }

    public void ResetMenu()
    {
        profedexCanvas.SetActive(false);
        inventarioCanvas.SetActive(false);
        mapaCanvas.SetActive(false);
        ajustesCanvas.SetActive(false);

        menuPrincipal.SetActive(true);
    }

    public void OpenProfedex()
    {
        ResetMenu();
        menuPrincipal.SetActive(false);
        profedexCanvas.SetActive(true);
    }

    public void OpenInventario()
    {
        ResetMenu();
        menuPrincipal.SetActive(false);
        inventarioCanvas.SetActive(true);
    }

    public void OpenMapa()
    {
        ResetMenu();
        menuPrincipal.SetActive(false);
        mapaCanvas.SetActive(true);
    }

    public void OpenAjustes()
    {
        ResetMenu();
        menuPrincipal.SetActive(false);
        ajustesCanvas.SetActive(true);
    }

    public void VolverMenuPrincipal()
    {
        ResetMenu();
    }

    public void SalirMenu()
    {
        ResetMenu();
        menuRoot.SetActive(false);
    }
}