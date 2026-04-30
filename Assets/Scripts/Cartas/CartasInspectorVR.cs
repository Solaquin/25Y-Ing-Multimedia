using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CartasInspectorVR : MonoBehaviour
{
    [Header("Referencias")]
    public InventarioCartas inventario;
    public GameObject cartaPrefab;
    public Transform puntoInspeccion;

    [Header("Ajustes visuales")]
    public float separacion = 0.15f;
    public float rotacionAbanico = 20f;
    public float inclinacion = 15f;

    [Header("VR Input")]
    public InputActionProperty joystickDerecho; // Vector2

    public float umbral = 0.5f; // sensibilidad
    private bool puedeMover = true;

    private List<GameObject> cartasVisuales = new List<GameObject>();
    private bool abierto = false;

    private int indiceActual = 0;

    public void ToggleInspeccion()
    {
        if (abierto)
            Cerrar();
        else
            Abrir();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleInspeccion();

        if (!abierto) return;

        //INPUT VR (joystick derecho)
        Vector2 input = joystickDerecho.action.ReadValue<Vector2>();

        if (puedeMover)
        {
            if (input.x > umbral)
            {
                Siguiente();
                puedeMover = false;
            }
            else if (input.x < -umbral)
            {
                Anterior();
                puedeMover = false;
            }
        }

        // 🔄 Reset cuando suelta el joystick
        if (Mathf.Abs(input.x) < 0.2f)
        {
            puedeMover = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
            Siguiente();

        if (Input.GetKeyDown(KeyCode.A))
            Anterior();
    }

    void Abrir()
    {

        abierto = true;
        indiceActual = Mathf.Clamp(1, 0, inventario.cartas - 1);

        MostrarCartas();
    }

    void Cerrar()
    {
        abierto = false;
        LimpiarCartas();
    }

    void MostrarCartas()
    {
        LimpiarCartas();
        Debug.Log("Mostrando carta index: " + indiceActual);
        int total = inventario.cartas;

        if (total <= 0) return;

        // 🔥 CASO 1: SOLO UNA CARTA
        if (total == 1)
        {
            GameObject carta = Instantiate(
                cartaPrefab,
                puntoInspeccion.position,
                puntoInspeccion.rotation,
                puntoInspeccion
            );

            // color
            Renderer rend = carta.GetComponentInChildren<Renderer>();
            if (rend != null && inventario.colores.Count > 0)
            {
                rend.material = new Material(rend.material);
                rend.material.color = inventario.colores[0];
            }

            cartasVisuales.Add(carta);

            // posición centrada
            PosicionarCarta(carta, 0);

            return;
        }

        // 🔥 CASO NORMAL (3 cartas)
        for (int i = -1; i <= 1; i++)
        {
            int index = indiceActual + i;

            if (index < 0 || index >= total)
                continue;

            GameObject carta = Instantiate(
                cartaPrefab,
                puntoInspeccion.position,
                puntoInspeccion.rotation,
                puntoInspeccion
            );

            Renderer rend = carta.GetComponentInChildren<Renderer>();
            if (rend != null && index < inventario.colores.Count)
            {
                rend.material = new Material(rend.material);
                rend.material.color = inventario.colores[index];
            }

            cartasVisuales.Add(carta);

            PosicionarCarta(carta, i);
        }
    }

    void PosicionarCarta(GameObject carta, int posicion)
    {
        float offsetX = posicion * separacion;
        float rotZ = -posicion * rotacionAbanico;

        carta.transform.localPosition = new Vector3(offsetX, 0, 0);

        carta.transform.LookAt(Camera.main.transform);
        carta.transform.Rotate(0, 90, 0);

        carta.transform.Rotate(0, 0,0);

        carta.transform.Rotate(inclinacion, 0, 0);

        if (posicion == 0)
            carta.transform.localScale = Vector3.one * 1.2f;
    }

    void Siguiente()
    {
        if (indiceActual < inventario.cartas - 1)
        {
            indiceActual++;
            MostrarCartas();
        }
    }

    void Anterior()
    {
        if (indiceActual > 0)
        {
            indiceActual--;
            MostrarCartas();
        }
    }

    void LimpiarCartas()
    {
        foreach (var c in cartasVisuales)
            Destroy(c);

        cartasVisuales.Clear();
    }
}