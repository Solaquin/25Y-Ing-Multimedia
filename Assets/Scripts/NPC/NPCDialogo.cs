using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NPCDialogo : MonoBehaviour
{
    [Header("Audio Interacción")]
    public AudioInteractivo audioInteractuar;

    [Header("Audio Dialogo (auto)")]
    public AudioSource audioDialogoSource;
    public AudioClip audioDialogoClip;

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

    private Quaternion rotacionOriginal;
    private bool rotacionGuardada = false;
    private bool siguiendoJugador = false;
    private bool volviendoRotacion = false;

    [Header("Input VR")]
    public InputActionReference botonA;

    [Header("Detección de mirada")]
    public Transform camaraJugador;
    [Range(0, 1)]
    public float umbralMirada = 0.7f;

    [Header("Animator")]
    public Animator animator;



    void Start()
    {
        botonHablar.SetActive(false);
        panelDialogo.SetActive(false);
        flechaContinuar.SetActive(false);

        nombreNPC.text = nombreDelNPC;

        if (botonA != null)
            botonA.action.Enable();

        if (camaraJugador == null)
            camaraJugador = Camera.main.transform;

        // 🔥 AUTO CONFIG AUDIO SOURCE (NO necesitas arrastrarlo)
        if (audioDialogoSource == null)
        {
            audioDialogoSource = GetComponent<AudioSource>();

            if (audioDialogoSource == null)
            {
                audioDialogoSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // 🔧 Configuración
        audioDialogoSource.loop = true;
        audioDialogoSource.playOnAwake = false;
        audioDialogoSource.spatialBlend = 0f; // 2D (mejor para diálogo)
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

            siguiendoJugador = false;
            volviendoRotacion = true;

            SetTalking(false);
            DetenerAudioDialogo(); // 🔴 importante
        }
    }

    void Update()
    {
        if (camaraJugador == null) return;

        if (siguiendoJugador)
        {
            SeguirMirandoJugador();
        }


        if (volviendoRotacion)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionOriginal,
                5f * Time.deltaTime
            );

            if (Quaternion.Angle(
                transform.rotation,
                rotacionOriginal) < 1f)
            {
                transform.rotation = rotacionOriginal;
                volviendoRotacion = false;
            }
        }

        bool mirandoNPC = EstaMirandoAlNPC();

        botonHablar.SetActive(jugadorCerca && mirandoNPC && !panelDialogo.activeSelf);

        // VR
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
        rotacionOriginal = transform.rotation;
        rotacionGuardada = true;
        siguiendoJugador = true;
        volviendoRotacion = false;

        SetTalking(true);
        botonHablar.SetActive(false);
        panelDialogo.SetActive(true);
        lineaActual = 0;

        StartCoroutine(EscribirTexto());
    }

    void SiguienteLinea()
    {
        if (escribiendo)
        {
            escribiendo = false; // ✅ señal para que la coroutine vieja se autodestruya
            StopAllCoroutines();
            textoDialogo.text = dialogos[lineaActual];
            flechaContinuar.SetActive(true);
            DetenerAudioDialogo();
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

            siguiendoJugador = false;
            volviendoRotacion = true;

            botonHablar.SetActive(false);
            DetenerAudioDialogo();
            SetTalking(false);
            OnDialogoTerminado?.Invoke();
        }
    }

    IEnumerator EscribirTexto()
    {
        escribiendo = true;
        flechaContinuar.SetActive(false);
        textoDialogo.text = "";

        string lineaAEscribir = dialogos[lineaActual]; // ✅ captura la línea al inicio

        if (audioDialogoClip != null)
        {
            audioDialogoSource.clip = audioDialogoClip;
            audioDialogoSource.Play();
        }

        foreach (char letra in lineaAEscribir)
        {
            if (!escribiendo) yield break; // ✅ si se canceló, salir limpio

            textoDialogo.text += letra;
            yield return new WaitForSeconds(0.03f);
        }

        escribiendo = false;
        flechaContinuar.SetActive(true);
        DetenerAudioDialogo();
    }

    void DetenerAudioDialogo()
    {
        if (audioDialogoSource != null && audioDialogoSource.isPlaying)
        {
            audioDialogoSource.Stop();
        }
    }

    public void Interactuar()
    {
        if (!jugadorCerca) return;

        if (audioInteractuar != null)
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

    void SetTalking(bool value)
    {
        if (animator != null)
            animator.SetBool("IsTalking", value);
    }

    void SeguirMirandoJugador()
    {
        if (camaraJugador == null)
            return;

        Vector3 direccion =
            camaraJugador.position - transform.position;

        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.01f)
            return;

        Quaternion objetivo =
            Quaternion.LookRotation(direccion) * Quaternion.Euler(0f, 180f, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            objetivo,
            5f * Time.deltaTime
        );
    }

    void RestaurarRotacion()
    {
        if (!rotacionGuardada)
            return;

        transform.rotation = rotacionOriginal;
        rotacionGuardada = false;
    }
}