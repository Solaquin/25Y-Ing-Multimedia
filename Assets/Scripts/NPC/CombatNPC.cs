using UnityEngine;

public class CombatNPC : MonoBehaviour
{
    [Header("Party del enemigo que usará en combate")]
    public NPCParty enemyParty;

    [Header("Diálogo alternativo después de ser derrotado (opcional)")]
    public string[] dialogosPostDerrota;

    private NPCDialogo npcDialogo;
    private bool yaFueDerrotado = false;

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
        // Si ya fue derrotado, el diálogo termina normalmente sin iniciar combate
        if (yaFueDerrotado)
        {
            npcDialogo.botonHablar.SetActive(true);
            return;
        }

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

        BattleTransitionManager.Instance.StartBattleTransition(enemyParty, this);
    }

    /// <summary>
    /// Llamado por BattleTransitionManager cuando el jugador gana.
    /// </summary>
    public void MarcarComoDerrotado()
    {
        yaFueDerrotado = true;

        // Cambiar a diálogos post-derrota si están asignados
        if (dialogosPostDerrota != null && dialogosPostDerrota.Length > 0)
            npcDialogo.dialogos = dialogosPostDerrota;
    }
}