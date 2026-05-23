using UnityEngine;

public class HealNPC : MonoBehaviour
{
    private NPCDialogo npcDialogo;
    private bool curando = false; // ✅ anti-spam igual que CombatNPC

    void Awake()
    {
        npcDialogo = GetComponent<NPCDialogo>();
        if (npcDialogo == null)
            Debug.LogError("HealNPC necesita un NPCDialogo en el mismo GameObject: " + gameObject.name);
    }

    void OnEnable()
    {
        npcDialogo.OnDialogoTerminado += HandleDialogoTerminado;
    }

    void OnDisable()
    {
        npcDialogo.OnDialogoTerminado -= HandleDialogoTerminado;
    }

    private void HandleDialogoTerminado()
    {
        if (curando) return; // ✅ evita curar varias veces

        if (PlayerPartyManager.Instance == null)
        {
            Debug.LogError("No se encontró PlayerPartyManager en la escena.");
            return;
        }

        curando = true;
        PlayerPartyManager.Instance.HealParty();
        Debug.Log("¡Party curada!");

        // ✅ Pequeño delay para que no se sienta instantáneo (opcional)
        Invoke(nameof(FinCura), 0.5f);
    }

    private void FinCura()
    {
        curando = false;
        npcDialogo.botonHablar.SetActive(false); // ocultar botón al terminar
    }
}
