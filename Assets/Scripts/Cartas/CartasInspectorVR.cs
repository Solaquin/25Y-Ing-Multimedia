using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class CartasInspectorVR : MonoBehaviour
{
    [Header("Referencias")]
    public InventarioCartas inventario;
    public GameObject cartaPrefab;
    public Camera camaraVR;

    [Header("Posición frente al jugador")]
    public float distancia = 0.6f;
    public float alturaOffset = -0.1f;

    [Header("Abanico")]
    public float separacion = 0.18f;
    public float rotacionAbanico = 12f;
    public float inclinacion = 10f;

    [Header("VR Input")]
    public InputActionProperty joystickDerecho;
    public float umbral = 0.5f;

    [Header("Movimiento VR")]
    public MonoBehaviour moveProvider;
    public MonoBehaviour turnProvider; 

    private List<GameObject> cartasVisuales = new List<GameObject>();
    private bool abierto = false;
    private bool puedeMover = true;
    private int indiceActual = 0;

    void Start()
    {
        if (camaraVR == null)
            camaraVR = GetComponent<Camera>();
    }

    public void ToggleInspeccion()
    {
        if (abierto) Cerrar();
        else Abrir();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ToggleInspeccion();
        if (!abierto) return;
        if (Input.GetKeyDown(KeyCode.D)) Siguiente();
        if (Input.GetKeyDown(KeyCode.A)) Anterior();

        if (joystickDerecho.action == null) return;
        Vector2 stick = joystickDerecho.action.ReadValue<Vector2>();
        if (puedeMover)
        {
            if (stick.x > umbral) { Siguiente(); puedeMover = false; }
            else if (stick.x < -umbral) { Anterior(); puedeMover = false; }
        }
        if (Mathf.Abs(stick.x) < 0.2f) puedeMover = true;
    }

    void Abrir()
    {
        if (inventario.cartas <= 0) return;
        if (abierto) return;
        indiceActual = Mathf.Clamp(indiceActual, 0, inventario.cartas - 1);
        abierto = true;
        // Bloquear movimiento
        if (moveProvider != null) moveProvider.enabled = false;
        if (turnProvider != null) turnProvider.enabled = false;
        MostrarCartas();
    }

    void Cerrar()
    {
        abierto = false;
        LimpiarCartas();

        // Restaurar movimiento
        if (moveProvider != null) moveProvider.enabled = true;
        if (turnProvider != null) turnProvider.enabled = true;
    }

    void MostrarCartas()
    {
        LimpiarCartas();
        int total = inventario.cartas;
        if (total <= 0) return;

        Transform cam = camaraVR.transform;
        Vector3 frente = cam.position
                       + cam.forward * distancia
                       + cam.up * alturaOffset;

        for (int offset = -1; offset <= 1; offset++)
        {
            int idx = indiceActual + offset;
            if (idx < 0 || idx >= total) continue;
            CrearCarta(idx, offset, frente, cam);
        }
    }

    void CrearCarta(int idx, int offset, Vector3 centro, Transform cam)
    {
        Vector3 pos = centro + cam.right * (offset * separacion);
        Quaternion rot = Quaternion.LookRotation(-cam.forward, cam.up)
                       * Quaternion.Euler(inclinacion, 90f, -offset * rotacionAbanico);

        GameObject carta = Instantiate(cartaPrefab, pos, rot);

        if (offset != 0)
            carta.transform.localScale *= 0.85f;

        // Aplicar material del ScriptableObject
        CartaSO datos = inventario.Cartas[idx];
        Renderer rend = carta.GetComponent<Renderer>()
                     ?? carta.GetComponentInChildren<Renderer>();
        if (rend != null && datos.materialCarta != null)
            rend.material = datos.materialCarta;

        cartasVisuales.Add(carta);
    }

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