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

    [Header("Profedex Areas")]
    public GameObject menuAreas;
    public GameObject area1Canvas;
    public GameObject area2Canvas;
    public GameObject area3Canvas;
    public GameObject area4Canvas;

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

        // 🔹 Reset de áreas
        DisableAreas();
        if (menuAreas != null)
            menuAreas.SetActive(false);
    }

    void DisableAreas()
    {
        if (area1Canvas != null) area1Canvas.SetActive(false);
        if (area2Canvas != null) area2Canvas.SetActive(false);
        if (area3Canvas != null) area3Canvas.SetActive(false);
        if (area4Canvas != null) area4Canvas.SetActive(false);
    }

    public void OpenProfedex()
    {
        ResetMenu();
        menuPrincipal.SetActive(false);
        profedexCanvas.SetActive(true);

        DisableAreas();
        if (menuAreas != null)
            menuAreas.SetActive(true);
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

    // 🔹 AREAS PROFEDEx

    public void OpenArea1()
    {
        DisableAreas();
        menuAreas.SetActive(false);
        area1Canvas.SetActive(true);
    }

    public void OpenArea2()
    {
        DisableAreas();
        menuAreas.SetActive(false);
        area2Canvas.SetActive(true);
    }

    public void OpenArea3()
    {
        DisableAreas();
        menuAreas.SetActive(false);
        area3Canvas.SetActive(true);
    }

    public void OpenArea4()
    {
        DisableAreas();
        menuAreas.SetActive(false);
        area4Canvas.SetActive(true);
    }

    public void VolverAreas()
    {
        DisableAreas();
        menuAreas.SetActive(true);
    }
}