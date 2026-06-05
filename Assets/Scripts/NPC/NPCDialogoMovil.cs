using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;

[System.Serializable]
public class PasoNPC
{
    [TextArea(2, 5)]
    public string[] dialogo;

    public Transform puntoMovimiento;

    public float velocidadMovimiento = 2f;

    public float esperaDespues = 0.5f;

    [Header("Rotación al llegar al punto")]
    public Vector3 rotacionFinal;          // rotación al llegar
    public float velocidadRotacion = 5f;

    [Header("GameObjects al terminar este paso")]
    public GameObject[] activarAlTerminar;

    public GameObject[] desactivarAlTerminar;
}

public class NPCDialogoMovil : MonoBehaviour
{
    [Header("UI")]
    public GameObject botonHablar;
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI nombreNPC;
    public GameObject flechaContinuar;

    [Header("Nombre")]
    public string nombreDelNPC = "NPC";
    public Animator animator;

    [Header("Secuencia")]
    public PasoNPC[] pasos;

    [Header("Input VR")]
    public InputActionReference botonA;

    public event Action OnDialogoTerminado;

    private int pasoActual = 0;
    private int lineaActual = 0;

    private bool escribiendo = false;
    private bool jugadorCerca = false;
    private bool ejecutando = false;

    string[] dialogos;

    [Header("Audio Dialogo (auto)")]
    public AudioSource audioDialogoSource;
    public AudioClip audioDialogoClip;

    // =========================
    // (FIX SISTEMA)
    // =========================
    private bool bloqueadoPorMovimiento = false;
    private bool esperandoPrimerDialogo = false;
    private Coroutine escrituraCoroutine;
    private Coroutine movimientoCoroutine;
    private bool secuenciaTerminada = false;

    [Header("Diálogo después de elegir starter")]
    public PasoNPC[] pasosDespuesStarter;

    private bool starterElegido = false;

    private bool siguiendoJugador = false;

    void Start()
    {
        starterElegido = false;
        botonHablar.SetActive(false);
        panelDialogo.SetActive(false);
        flechaContinuar.SetActive(false);

        nombreNPC.text = nombreDelNPC;

        if (botonA != null)
            botonA.action.Enable();

        if (audioDialogoSource == null)
        {
            audioDialogoSource = GetComponent<AudioSource>();

            if (audioDialogoSource == null)
            {
                audioDialogoSource = gameObject.AddComponent<AudioSource>();
            }
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
            siguiendoJugador = false;


            botonHablar.SetActive(false);
            panelDialogo.SetActive(false);
            SetTalking(false);
            DetenerAudioDialogo();
        }
    }

    void Update()
    {

        if (siguiendoJugador)
        {
            SeguirMirandoJugador();
        }

        // VR input
        if (jugadorCerca && botonA != null && botonA.action.WasPressedThisFrame())
            Interactuar();
    }

    // =========================
    // 🎮 INTERACCIÓN
    // =========================

    public void Interactuar()
    {
        if (!jugadorCerca) return;

        botonHablar.SetActive(false);

        if (bloqueadoPorMovimiento) return;

        // 🎯 SI está escribiendo → SKIP INSTANTÁNEO
        if (escribiendo)
        {
            if (escrituraCoroutine != null)
                StopCoroutine(escrituraCoroutine);

            textoDialogo.text = dialogos[lineaActual];
            escribiendo = false;
            flechaContinuar.SetActive(true);
            DetenerAudioDialogo();
            return;
        }

        if (!panelDialogo.activeSelf)
        {
            // 🧠 si ya terminó, NO reinicia
            if (secuenciaTerminada)
            {
                // opcional: repetir último diálogo
                pasoActual = pasos.Length - 1;
                lineaActual = 0;

                dialogos = pasos[pasoActual].dialogo;

                panelDialogo.SetActive(true);

                if (escrituraCoroutine != null)
                    StopCoroutine(escrituraCoroutine);

                escrituraCoroutine = StartCoroutine(EscribirTexto());

                return;
            }

            if(!ejecutando)
            {

                IniciarSecuencia();
            }
            else
            {
                panelDialogo.SetActive(true);
                if (escrituraCoroutine != null)
                    StopCoroutine(escrituraCoroutine);
                escrituraCoroutine = StartCoroutine(EscribirTexto());
            }
        }
        else
        {
            SiguienteLinea();
        }
    }
    // =========================
    // 🎬 SECUENCIA
    // =========================

    public void IniciarSecuencia()
    {
        if (pasos == null || pasos.Length == 0) return;

        ejecutando = true;
        pasoActual = 0;

        EjecutarPaso();
    }

    void EjecutarPaso()
    {
        if (pasoActual >= pasos.Length)
        {
            Terminar();
            return;
        }

        dialogos = pasos[pasoActual].dialogo;
        lineaActual = 0;

        siguiendoJugador = true;

        botonHablar.SetActive(false);
        panelDialogo.SetActive(true);

        // 🚫 evita spam de escritura
        if (escrituraCoroutine != null)
            StopCoroutine(escrituraCoroutine);

        escrituraCoroutine = StartCoroutine(EscribirTexto());
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
            if (escrituraCoroutine != null)
                StopCoroutine(escrituraCoroutine);

            escrituraCoroutine = StartCoroutine(EscribirTexto());
        }
        else
        {
            SetTalking(false);

            siguiendoJugador = false;

            panelDialogo.SetActive(false);
            bloqueadoPorMovimiento = true;

            if (movimientoCoroutine != null)
                StopCoroutine(movimientoCoroutine);

            movimientoCoroutine = StartCoroutine(MoverYContinuar());
        }
    }

    IEnumerator EscribirTexto()
    {
        SetWalking(false);
        SetTalking(true);

        escribiendo = true;

        flechaContinuar.SetActive(false);
        textoDialogo.text = "";

        if (audioDialogoClip != null)
        {
            audioDialogoSource.clip = audioDialogoClip;
            audioDialogoSource.Play();
        }

        foreach (char c in dialogos[lineaActual])
        {
            textoDialogo.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        escribiendo = false;
        flechaContinuar.SetActive(true);
        DetenerAudioDialogo();

        // 🔓 desbloqueo del primer diálogo si aplica
        if (esperandoPrimerDialogo)
        {
            bloqueadoPorMovimiento = false;
            esperandoPrimerDialogo = false;
        }

        // 🔓 desbloqueo después del primer texto del nuevo paso
        if (esperandoPrimerDialogo)
        {
            bloqueadoPorMovimiento = false;
            esperandoPrimerDialogo = false;
        }
    }

    IEnumerator MoverYContinuar()
    {
        siguiendoJugador = false;

        PasoNPC paso = pasos[pasoActual];

        // 🔹 Rotación antes de moverse
        if (paso.puntoMovimiento != null)
        {
            // Calcula la rotación mirando hacia el destino
            Vector3 direccion = (paso.puntoMovimiento.position - transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                Quaternion rotHaciaDestino = Quaternion.LookRotation(direccion, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);

                while (Quaternion.Angle(transform.rotation, rotHaciaDestino) > 0.5f)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        rotHaciaDestino,
                        paso.velocidadRotacion * Time.deltaTime
                    );
                    yield return null;
                }
            }
        }

        // 🔹 Movimiento hacia el punto
        if (paso.puntoMovimiento != null)
        {
            SetTalking(false);
            SetWalking(true);

            while (Vector3.Distance(transform.position, paso.puntoMovimiento.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    paso.puntoMovimiento.position,
                    paso.velocidadMovimiento * Time.deltaTime
                );

                yield return null;
            }

            SetWalking(false);
        }

        // 🔹 Rotación al llegar
        if (paso.rotacionFinal != Vector3.zero)
        {
            Quaternion rotFinal = Quaternion.Euler(paso.rotacionFinal);

            while (Quaternion.Angle(transform.rotation, rotFinal) > 0.5f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rotFinal,
                    paso.velocidadRotacion * Time.deltaTime
                );

                yield return null;
            }

            transform.rotation = rotFinal;
        }

        yield return new WaitForSeconds(paso.esperaDespues);

        AplicarGameObjects();
        pasoActual++;

        // 🔓 desbloqueo controlado
        bloqueadoPorMovimiento = false;
        esperandoPrimerDialogo = true;

        EjecutarPaso();
    }

    void AplicarGameObjects()
    {
        PasoNPC paso = pasos[pasoActual];

        if (paso.activarAlTerminar != null)
        {
            foreach (GameObject go in paso.activarAlTerminar)
            {
                if (go != null)
                    go.SetActive(true);
            }
        }

        if (paso.desactivarAlTerminar != null)
        {
            foreach (GameObject go in paso.desactivarAlTerminar)
            {
                if (go != null)
                    go.SetActive(false);
            }
        }
    }

    public void ActivarDialogoStarter()
    {
        if (starterElegido) return;

        starterElegido = true;

        // Cambiamos el diálogo a la nueva secuencia
        pasos = pasosDespuesStarter;

        // Opcional: arranca automáticamente el diálogo
        if (!ejecutando)
        {
            IniciarSecuencia();
        }
    }

    void Terminar()
    {
        ejecutando = false;
        panelDialogo.SetActive(false);

        secuenciaTerminada = true; // 👈 IMPORTANTE

        SetTalking(false);
        SetWalking(false);
        if (jugadorCerca)
        {
            botonHablar.SetActive(true);
        }
        DetenerAudioDialogo();

        OnDialogoTerminado?.Invoke();
    }

    void SetTalking(bool value)
    {
        if (animator != null)
            animator.SetBool("IsTalking", value);
    }

    void SetWalking(bool value)
    {
        if (animator != null)
            animator.SetBool("IsWalking", value);
    }

    void SeguirMirandoJugador()
    {
        if (Camera.main == null)
            return;

        Vector3 direccion =
            Camera.main.transform.position - transform.position;

        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.01f)
            return;

        Quaternion objetivo =
            Quaternion.LookRotation(direccion)
            * Quaternion.Euler(0f, 180f, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            objetivo,
            5f * Time.deltaTime
        );
    }

    void DetenerAudioDialogo()
    {
        if (audioDialogoSource != null && audioDialogoSource.isPlaying)
        {
            audioDialogoSource.Stop();
        }
    }
}