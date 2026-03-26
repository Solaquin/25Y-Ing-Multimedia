using UnityEngine;

public class NPCShop : MonoBehaviour
{
    public GameObject botonTienda;
    public GameObject panelShop;

    void Start()
    {
        botonTienda.SetActive(false);
        panelShop.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            botonTienda.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            botonTienda.SetActive(false);
            panelShop.SetActive(false);
        }
    }

    public void AbrirTienda()
    {
        panelShop.SetActive(true);
        botonTienda.SetActive(false);
    }

    private void Update()
    {
        if (panelShop.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            panelShop.SetActive(false);
            botonTienda.SetActive(true);
        }
    }
}

