using UnityEngine;
using System;

/// <summary>
/// Singleton que maneja el dinero del jugador.
/// Es la única fuente de verdad del saldo — ningún otro script
/// debe guardar dinero localmente.
/// El SaveManager leerá y escribirá directamente aquí.
/// </summary>
public class WalletSystem : MonoBehaviour
{
    public static WalletSystem Instance { get; private set; }

    [SerializeField] private int dineroInicial = 500;

    private int dinero;

    /// <summary>
    /// Saldo actual del jugador. Solo lectura desde fuera.
    /// </summary>
    public int Dinero => dinero;

    /// <summary>
    /// Se dispara cada vez que el saldo cambia. La UI se suscribe aquí.
    /// </summary>
    public event Action<int> OnDineroChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        dinero = dineroInicial;
    }

    /// <summary>
    /// Intenta gastar una cantidad. Devuelve true si había suficiente saldo.
    /// </summary>
    public bool Gastar(int cantidad)
    {
        if (cantidad <= 0)
        {
            Debug.LogWarning("[Wallet] Cantidad inválida: " + cantidad);
            return false;
        }

        if (dinero < cantidad)
        {
            NotificationManager.Send("¡No tienes sufiente dinero!");
            return false;
        }

        dinero -= cantidad;
        OnDineroChanged?.Invoke(dinero);
        Debug.Log($"[Wallet] -{cantidad} | Saldo: {dinero}");
        return true;
    }

    /// <summary>
    /// Añade dinero al saldo (recompensas, victorias en combate, etc.)
    /// </summary>
    public void Ganar(int cantidad)
    {
        if (cantidad <= 0) return;

        dinero += cantidad;
        OnDineroChanged?.Invoke(dinero);
        Debug.Log($"[Wallet] +{cantidad} | Saldo: {dinero}");
    }

    /// <summary>
    /// Usado exclusivamente por el SaveManager al cargar una partida.
    /// </summary>
    public void CargarSaldo(int cantidad)
    {
        dinero = Mathf.Max(0, cantidad);
        OnDineroChanged?.Invoke(dinero);
    }
}