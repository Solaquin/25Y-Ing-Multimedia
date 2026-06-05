using UnityEngine;

public class CombatNPC : MonoBehaviour
{
    [Header("Party del enemigo que usará en combate")]
    public NPCParty enemyParty;

    [Header("Diálogo alternativo después de ser derrotado (opcional)")]
    public string[] dialogosPostDerrota;

    private NPCDialogo npcDialogo;
    private bool yaFueDerrotado = false;
    private bool enCombate = false; //bandera anti-spam

    [Header("Recompensas")]
    public int dineroOtorgado = 100;
    public CartaSO cartaRecompensa;
    private bool recompensaEntregada;

    public int DineroOtorgado => dineroOtorgado;
    public CartaSO CartaRecompensa => cartaRecompensa;

    void Awake()
    {
        npcDialogo = GetComponent<NPCDialogo>();
        if (npcDialogo == null)
            Debug.LogError("CombatNPC necesita un NPCDialogo en el mismo GameObject: " + gameObject.name);
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
        if (yaFueDerrotado)
        {
            npcDialogo.botonHablar.SetActive(true);
            return;
        }

        // Si ya hay una batalla en curso, ignorar
        if (enCombate) return;

        if (BattleTransitionManager.Instance == null)
        {
            Debug.LogError("No se encontró BattleTransitionManager en la escena.");
            return;
        }

        if (enemyParty == null)
        {
            Debug.LogError("CombatNPC no tiene asignada una NPCParty: " + gameObject.name);
            return;
        }

        enCombate = true; //bloquear antes de iniciar
        BattleTransitionManager.Instance.StartBattleTransition(enemyParty, this);
    }

    public void MarcarComoDerrotado()
    {
        yaFueDerrotado = true;
        enCombate = false; // resetear por si acaso

        if (dialogosPostDerrota != null && dialogosPostDerrota.Length > 0)
            npcDialogo.dialogos = dialogosPostDerrota;
    }

    // Llamar esto si la batalla se cancela o falla, para no quedar bloqueado
    public void CancelarCombate()
    {
        enCombate = false;
    }

    public void EntregarRecompensas()
    {
        if (recompensaEntregada)
            return;

        recompensaEntregada = true;

        WalletSystem.Instance.Ganar(dineroOtorgado);

        if (cartaRecompensa != null)
            InventarioCartas.Instance.AgregarCarta(cartaRecompensa);
    }
}