using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Monta este script en la Main Camera.
/// No necesita puntoInspeccion externo — calcula la posición frente al jugador en runtime.
/// </summary>
public class CartasInspectorVR : MonoBehaviour
{
    [Header("Referencias")]
    public InventarioCartas inventario;
    public GameObject cartaPrefab;

    [Header("Posición frente al jugador")]
    public float distancia = 0.6f;   // metros frente a la cámara
    public float alturaOffset = -0.1f; // bajar un poco respecto al centro

    [Header("Abanico")]
    public float separacion = 0.18f;  // distancia entre cartas
    public float rotacionAbanico = 12f;    // grados de inclinación lateral
    public float inclinacion = 10f;    // grados hacia el jugador (X)

    [Header("Escala")]
    public float escalaCentral = 0.08f;   // ajusta según tamaño de tu prefab
    public float escalaLateral = 0.065f;

    [Header("VR Input")]
    public InputActionProperty joystickDerecho;
    public float umbral = 0.5f;

    // estado
    private List<GameObject> cartasVisuales = new List<GameObject>();
    private bool abierto = false;
    private bool puedeMover = true;
    private int indiceActual = 0;

    // ───────────────────────────────────────────
    public void ToggleInspeccion()
    {
        if (abierto) Cerrar();
        else Abrir();
    }

    void Update()
    {
        // debug teclado
        if (Input.GetKeyDown(KeyCode.E)) ToggleInspeccion();
        if (!abierto) return;
        if (Input.GetKeyDown(KeyCode.D)) Siguiente();
        if (Input.GetKeyDown(KeyCode.A)) Anterior();

        // joystick VR
        if (joystickDerecho.action == null) return;
        Vector2 stick = joystickDerecho.action.ReadValue<Vector2>();
        if (puedeMover)
        {
            if (stick.x > umbral) { Siguiente(); puedeMover = false; }
            else if (stick.x < -umbral) { Anterior(); puedeMover = false; }
        }
        if (Mathf.Abs(stick.x) < 0.2f) puedeMover = true;
    }

    // ───────────────────────────────────────────
    void Abrir()
    {
        if (inventario.cartas <= 0) return;
        indiceActual = Mathf.Clamp(indiceActual, 0, inventario.cartas - 1);
        abierto = true;
        MostrarCartas();
    }

    void Cerrar()
    {
        abierto = false;
        LimpiarCartas();
    }

    // ───────────────────────────────────────────
    void MostrarCartas()
    {
        LimpiarCartas();

        int total = inventario.cartas;
        if (total <= 0) return;

        // punto frente a la cámara calculado en runtime
        Vector3 frente = transform.position
                       + transform.forward * distancia
                       + transform.up * alturaOffset;

        // offsets a mostrar: solo los que existan
        // siempre mostramos: indice-1, indice, indice+1
        for (int offset = -1; offset <= 1; offset++)
        {
            int idx = indiceActual + offset;
            if (idx < 0 || idx >= total) continue;
            CrearCarta(idx, offset, frente);
        }
    }

    void CrearCarta(int idx, int offset, Vector3 centroMundo)
    {
        // posición: desplazamiento lateral en el eje derecho de la cámara
        Vector3 pos = centroMundo + transform.right * (offset * separacion);

        // rotación: la carta mira hacia atrás (hacia el jugador) + abanico en Z
        Quaternion rot = Quaternion.LookRotation(-transform.forward, transform.up)
               * Quaternion.Euler(inclinacion, 90f, -offset * rotacionAbanico);

        GameObject carta = Instantiate(cartaPrefab, pos, rot);

        // escala
        if (offset != 0)
            carta.transform.localScale *= 0.85f;

        // color
        Renderer rend = carta.GetComponent<Renderer>()
                     ?? carta.GetComponentInChildren<Renderer>();
        if (rend != null && idx < inventario.colores.Count)
        {
            rend.material = new Material(rend.material);
            rend.material.color = inventario.colores[idx];
        }

        cartasVisuales.Add(carta);
    }

    // ───────────────────────────────────────────
    void Siguiente()
    {
        if (indiceActual < inventario.cartas - 1)
        { indiceActual++; MostrarCartas(); }
    }

    void Anterior()
    {
        if (indiceActual > 0)
        { indiceActual--; MostrarCartas(); }
    }

    void LimpiarCartas()
    {
        foreach (var c in cartasVisuales) Destroy(c);
        cartasVisuales.Clear();
    }
}