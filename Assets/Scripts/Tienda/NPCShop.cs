using UnityEngine;

public class NPCShop : MonoBehaviour
{
    public GameObject botonTienda;
    public GameObject panelShop;

    [SerializeField] private ShopSystem shopSystem;

    // 🔊 NUEVO
    [Header("Audio")]
    public AudioInteractivo audioUI;

    void Start()
    {
        botonTienda.SetActive(false);
        panelShop.SetActive(false);
    }

    // 🔊 NUEVO
    void PlayClick()
    {
        if (audioUI != null)
            audioUI.ActivarAudio();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            botonTienda.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            botonTienda.SetActive(false);
            CerrarTienda();
        }
    }

    public void AbrirTienda()
    {
        PlayClick(); // 🔊 NUEVO

        panelShop.SetActive(true);
        botonTienda.SetActive(false);
        shopSystem.AbrirTienda();
    }

    public void CerrarTienda()
    {
        if (!panelShop.activeSelf) return;

        PlayClick(); // 🔊 NUEVO

        shopSystem.CerrarTienda();
        panelShop.SetActive(false);
        botonTienda.SetActive(true);
    }

    private void Update()
    {
        if (panelShop.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            PlayClick(); // 🔊 NUEVO
            CerrarTienda();
        }
    }
}