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

    void Start()
    {
        botonHablar.SetActive(false);
        panelDialogo.SetActive(false);
        flechaContinuar.SetActive(false);

        nombreNPC.text = nombreDelNPC;

        if (botonA != null)
            botonA.action.Enable();
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

        if (!panelDialogo.activeSelf)
        {
            IniciarSecuencia();
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

        panelDialogo.SetActive(true);

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
            StartCoroutine(MoverYContinuar());
        }
    }

    IEnumerator EscribirTexto()
    {
        escribiendo = true;

        flechaContinuar.SetActive(false);
        textoDialogo.text = "";

        foreach (char c in dialogos[lineaActual])
        {
            textoDialogo.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        escribiendo = false;
        flechaContinuar.SetActive(true);
    }

    IEnumerator MoverYContinuar()
    {
        PasoNPC paso = pasos[pasoActual];

        if (paso.puntoMovimiento != null)
        {
            while (Vector3.Distance(transform.position, paso.puntoMovimiento.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    paso.puntoMovimiento.position,
                    paso.velocidadMovimiento * Time.deltaTime
                );

                yield return null;
            }
        }

        yield return new WaitForSeconds(paso.esperaDespues);
        AplicarGameObjects();
        pasoActual++;

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
    void Terminar()
    {
        ejecutando = false;
        panelDialogo.SetActive(false);

        OnDialogoTerminado?.Invoke();
    }
}