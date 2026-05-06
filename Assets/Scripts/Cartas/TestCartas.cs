using UnityEngine;

// Solo para probar en editor — asigna cartas de prueba en el Inspector
public class TestCartas : MonoBehaviour
{
    public InventarioCartas inventario;
    public CartaSO[] cartasDePrueba; // arrastra tus ScriptableObjects aquí

    private int testIndex = 0;

    void Update()
    {
        // Space = dar la siguiente carta de prueba
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cartasDePrueba.Length == 0) return;
            CartaSO carta = cartasDePrueba[testIndex % cartasDePrueba.Length];
            inventario.AgregarCarta(carta);
            testIndex++;
        }
    }
}
