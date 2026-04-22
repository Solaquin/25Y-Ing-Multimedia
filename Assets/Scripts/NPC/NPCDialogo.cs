using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NPCDialogo : MonoBehaviour
{
    [Header("Audio")]
    public AudioInteractivo audioInteractuar;
    [Header("UI")]
    public GameObject botonHablar;
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI nombreNPC;
    public GameObject flechaContinuar;

    [Header("Dialogo")]
    public string[] dialogos;
    public string nombreDelNPC = "Profesor";

    public event Action OnDialogoTerminado;

    private int lineaActual = 0;
    private bool escribiendo = false;
    private bool jugadorCerca = false;

    [Header("Input VR")]
    public InputActionReference botonA;

    [Header("Detección de mirada")]
    public Transform camaraJugador;
    [Range(0, 1)]
    public float umbralMirada = 0.7f; // entre más alto, más preciso

    void Start()
    {
        botonHablar.SetActive(false);
        panelDialogo.SetActive(false);
        flechaContinuar.SetActive(false);

        nombreNPC.text = nombreDelNPC;

        if (botonA != null)
            botonA.action.Enable();

        if (camaraJugador == null)
        {
            camaraJugador = Camera.main.transform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            botonHablar.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            botonHablar.SetActive(false);
            panelDialogo.SetActive(false);
        }
    }

    void Update()
    {
        if (camaraJugador == null) return;

        bool mirandoNPC = EstaMirandoAlNPC();

        // Mostrar botón SOLO si está cerca Y mirando
        botonHablar.SetActive(jugadorCerca && mirandoNPC && !panelDialogo.activeSelf);

        // Interacción VR
        if (jugadorCerca && mirandoNPC && botonA != null && botonA.action.WasPressedThisFrame())
        {
            Interactuar();
        }

        // Teclado
        if (jugadorCerca && mirandoNPC && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interactuar();
        }
    }

    void IniciarDialogo()
    {
        botonHablar.SetActive(false);
        panelDialogo.SetActive(true);
        lineaActual = 0;

        StartCoroutine(EscribirTexto());
    }

    void SiguienteLinea()
    {
        if (escribiendo)
        {
            StopAllCoroutines();
            textoDialogo.text = dialogos[lineaActual];
            escribiendo = false;
            flechaContinuar.SetActive(true);
            return;
        }

        lineaActual++;

        if (lineaActual < dialogos.Length)
        {
            StartCoroutine(EscribirTexto());
        }
        else
        {
            panelDialogo.SetActive(false);
            botonHablar.SetActive(false);

            OnDialogoTerminado?.Invoke();
        }
    }

    IEnumerator EscribirTexto()
    {
        escribiendo = true;
        flechaContinuar.SetActive(false);
        textoDialogo.text = "";

        foreach (char letra in dialogos[lineaActual])
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(0.03f);
        }

        escribiendo = false;
        flechaContinuar.SetActive(true);
    }

    public void Interactuar()
    {
        if (!jugadorCerca) return;

        if(audioInteractuar != null)
        {
            audioInteractuar.ActivarAudio();
        }
        if (!panelDialogo.activeSelf)
        {
            IniciarDialogo();
        }
        else
        {
            SiguienteLinea();
        }
    }

    bool EstaMirandoAlNPC()
    {
        Vector3 direccionAlNPC = (transform.position - camaraJugador.position).normalized;
        float dot = Vector3.Dot(camaraJugador.forward, direccionAlNPC);

        return dot > umbralMirada;
    }
}
