using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public GameObject storageCanvas;

    [Header("Audio UI)")]
    public AudioInteractivo audioUI;

    [Header("Profedex Database")]
    public ProfedexDatabase database;

    [Header("Profedex Areas")]
    public GameObject menuAreas;
    public GameObject area1Canvas;
    public GameObject area2Canvas;
    public GameObject area3Canvas;
    public GameObject area4Canvas;

    [Header("Profemon Detalles")]
    public GameObject CanvasProfemonDetalle;

    public Image detalleImagen;

    public TMP_Text detalleNombre;
    public TMP_Text detalleFrase;
    public TMP_Text detalleDescripcion;
    public TMP_Text detalleAnecdota;

    public Image detalleDonde;
    public TMP_Text detalleDondeTexto;

    [Header("Navegación Profedex")]
    public TMP_Text numeroProfesor;

    private ProfemonData[] allProfessors;
    private int currentIndex;
    private ProfemonData currentData;

    [Header("Profedex Unknown")]
    public Sprite unknownSprite;
    public Sprite unknownLocationSprite;

    [Header("Progreso Profedex")]
    public TMP_Text registradosText;
    public TMP_Text registradosArea1;
    public TMP_Text registradosArea2;
    public TMP_Text registradosArea3;
    public TMP_Text registradosArea4;
    public TMP_Text registradosDetalle;

    [Header("Party UI")]
    public PartyMenuManager partyMenuManager;

    [Header("Storage UI")]
    public StorageMenuManager storageMenuManager;

    void Start()
    {
        ResetMenu();
    }
    void PlayClick()
    {
        if (audioUI != null)
            audioUI.ActivarAudio();
    }

    public void ResetMenu()
    {
        profedexCanvas.SetActive(false);
        inventarioCanvas.SetActive(false);
        mapaCanvas.SetActive(false);
        ajustesCanvas.SetActive(false);
        CanvasProfemonDetalle.SetActive(false);
        storageCanvas.SetActive(false);

        menuPrincipal.SetActive(true);

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
        PlayClick();
        ResetMenu();
        menuPrincipal.SetActive(false);
        profedexCanvas.SetActive(true);

        DisableAreas();

        if (menuAreas != null)
            menuAreas.SetActive(true);
        UpdateProfedexProgress();
    }

    public void OpenInventario()
    {
        PlayClick();
        ResetMenu();
        menuPrincipal.SetActive(false);
        inventarioCanvas.SetActive(true);

        if (partyMenuManager != null)
            partyMenuManager.RefreshParty();
    }

    public void OpenMapa()
    {
        PlayClick();
        ResetMenu();
        menuPrincipal.SetActive(false);
        mapaCanvas.SetActive(true);
    }

    public void OpenAjustes()
    {
        PlayClick();
        ResetMenu();
        menuPrincipal.SetActive(false);
        ajustesCanvas.SetActive(true);
    }

    public void VolverMenuPrincipal()
    {
        PlayClick();
        ResetMenu();
    }

    public void SalirMenu()
    {
        PlayClick();
        ResetMenu();
        menuRoot.SetActive(false);
    }

    // AREAS PROFEDEx

    public void OpenArea1()
    {
        PlayClick();
        DisableAreas();
        menuAreas.SetActive(false);
        area1Canvas.SetActive(true);
    }

    public void OpenArea2()
    {
        PlayClick();
        DisableAreas();
        menuAreas.SetActive(false);
        area2Canvas.SetActive(true);
    }

    public void OpenArea3()
    {
        PlayClick();
        DisableAreas();
        menuAreas.SetActive(false);
        area3Canvas.SetActive(true);
    }

    public void OpenArea4()
    {
        PlayClick();
        DisableAreas();
        menuAreas.SetActive(false);
        area4Canvas.SetActive(true);
    }

    public void OpenStorage()

    {
        PlayClick();
        ResetMenu();
        menuPrincipal.SetActive(false);
        storageCanvas.SetActive(true);

        if (storageMenuManager != null)
            storageMenuManager.Refresh();

    }

    public void VolverAreas()
    {
        PlayClick();
        DisableAreas();
        menuAreas.SetActive(true);
    }

    // DETALLE PROFESOR

    public void OpenProfesorDetalle(ProfemonData data)
    {
        PlayClick();
        DisableAreas();
        CanvasProfemonDetalle.SetActive(true);

        currentData = data;
        currentIndex = database.allProfemons.IndexOf(data);

        bool discovered = ProfedexManager.Instance.IsRegistered(data);

        // Número estilo pokedex
        numeroProfesor.text = "#" + (currentIndex + 1).ToString("000");

        if (discovered)
        {
            detalleImagen.sprite = data.image;
            detalleNombre.text = data.professorName;
            detalleFrase.text = data.phrase;
            detalleDescripcion.text = data.description;
            detalleAnecdota.text = data.anecdote;

            detalleDonde.sprite = data.whereToFindImage;
            detalleDondeTexto.text = data.whereToFind;
        }
        else
        {
            detalleImagen.sprite = unknownSprite;

            detalleNombre.text = "????";
            detalleFrase.text = "????";
            detalleDescripcion.text = "????";
            detalleAnecdota.text = "????";

            detalleDonde.sprite = unknownLocationSprite;
            detalleDondeTexto.text = "????";
        }
    }
    public void NextProfessor()
    {
        PlayClick();
        currentIndex++;

        if (currentIndex >= database.allProfemons.Count)
            currentIndex = 0;

        OpenProfesorDetalle(database.allProfemons[currentIndex]);
    }
    public void PreviousProfessor()
    {
        PlayClick();
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = database.allProfemons.Count - 1;

        OpenProfesorDetalle(database.allProfemons[currentIndex]);
    }

    public void CloseProfesorDetalle()
    {
        PlayClick();
        CanvasProfemonDetalle.SetActive(false);
        VolverAreas();
    }

    void UpdateProfedexProgress()
    {

        int total = database.allProfemons.Count;
        int registrados = 0;

        foreach (var prof in database.allProfemons)
        {
            if (ProfedexManager.Instance.IsRegistered(prof))
                registrados++;
        }

        registradosText.text = "Registrados: " + registrados + " / " + total;
        registradosArea1.text = "Registrados: " + registrados + " / " + total;
        registradosArea2.text = "Registrados: " + registrados + " / " + total;
        registradosArea3.text = "Registrados: " + registrados + " / " + total;
        registradosArea4.text = "Registrados: " + registrados + " / " + total;
        registradosDetalle.text = "Registrados: " + registrados + " / " + total;
    }
}