using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NPCDialogo : MonoBehaviour
{
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
        
        if (jugadorCerca && botonA != null && botonA.action.WasPressedThisFrame())
        {
            Interactuar();
        }

        // Opcional: también puedes probar con teclado para simulador
        if (jugadorCerca && Keyboard.current.spaceKey.wasPressedThisFrame)
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

        if (!panelDialogo.activeSelf)
        {
            IniciarDialogo();
        }
        else
        {
            SiguienteLinea();
        }
    }
}
