using UnityEngine;

public class NPCShop : MonoBehaviour
{
    public GameObject botonTienda;
    public GameObject panelShop;

    [SerializeField]private ShopSystem shopSystem;

    void Start()
    {
        botonTienda.SetActive(false);
        panelShop.SetActive(false);
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
        panelShop.SetActive(true);
        botonTienda.SetActive(false);
        shopSystem.AbrirTienda();
    }

    public void CerrarTienda()
    {
        if (!panelShop.activeSelf) return;

        shopSystem.CerrarTienda();
        panelShop.SetActive(false);
        botonTienda.SetActive(true);
    }

    private void Update()
    {
        if (panelShop.activeSelf && Input.GetKeyDown(KeyCode.E))
            CerrarTienda();
    }
}