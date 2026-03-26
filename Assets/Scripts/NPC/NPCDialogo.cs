using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class NPCDialogo : MonoBehaviour
{
    public GameObject botonHablar;
    public GameObject panelDialogo;

    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI nombreNPC;

    public GameObject flechaContinuar;

    public string[] dialogos;
    public string nombreDelNPC = "Profesor";

    // --- NUEVO: otros scripts se suscriben aquí para saber cuando termina el diálogo ---
    public event Action OnDialogoTerminado;

    private int lineaActual = 0;
    private bool escribiendo = false;
    private bool jugadorCerca = false;

    void Start()
    {
        botonHablar.SetActive(false);
        panelDialogo.SetActive(false);
        flechaContinuar.SetActive(false);

        nombreNPC.text = nombreDelNPC;
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
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
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
            // Cerrar panel
            panelDialogo.SetActive(false);
            botonHablar.SetActive(false); // no mostrar botón si va al combate

            // --- NUEVO: disparar evento para que CombatNPC inicie el combate ---
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
}