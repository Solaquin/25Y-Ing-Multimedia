using System.Collections;
using UnityEngine;

public class BattleTransitionManager : MonoBehaviour
{
    public static BattleTransitionManager Instance { get; private set; }

    [Header("Referencias")]
    public BattleSystem battleSystem;       // El GameObject del BattleSystem (desactivado al inicio)
    public Transform battleZoneSpawnPoint;  // Punto donde aparece el jugador al entrar al combate
    public Transform worldSpawnPoint;       // Punto fijo donde vuelve el jugador al salir del combate
    public GameObject xrRig;               // Tu XR Origin / XR Rig

    public GameObject battleCanva;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;     // El CanvasGroup del panel negro
    public float fadeDuration = 0.5f;

    private NPCParty pendingEnemyParty;
    private CombatNPC pendingCombatNPC;    // El NPC que inició el combate actual

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Llamado por CombatNPC al terminar el diálogo.
    /// </summary>
    public void StartBattleTransition(NPCParty enemyParty, CombatNPC combatNPC)
    {
        pendingEnemyParty = enemyParty;
        pendingCombatNPC = combatNPC;
        StartCoroutine(TransitionToBattle());
    }

    IEnumerator TransitionToBattle()
    {
        // 1. Fade a negro
        yield return StartCoroutine(Fade(0f, 1f));

        // 2. Teletransportar XR Rig a la zona de combate
        xrRig.transform.position = battleZoneSpawnPoint.position;
        xrRig.transform.rotation = battleZoneSpawnPoint.rotation;

        // 3. Activar BattleSystem y configurar el combate
        battleSystem.gameObject.SetActive(true);
        battleCanva.SetActive(true);
        battleSystem.SetupBattle(pendingEnemyParty);

        // 4. Fade de vuelta
        yield return StartCoroutine(Fade(1f, 0f));

    }

    /// <summary>
    /// Llamado por BattleSystem al terminar el combate.
    /// playerWon = true si el jugador ganó, false si perdió.
    /// </summary>
    public void EndBattleTransition(bool playerWon)
    {
        StartCoroutine(TransitionToWorld(playerWon));
    }

    IEnumerator TransitionToWorld(bool playerWon)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        // Desactivar BattleSystem
        battleSystem.gameObject.SetActive(false);

        // Volver al mundo (posición fija asignada en el Inspector)
        xrRig.transform.position = worldSpawnPoint.position;
        xrRig.transform.rotation = worldSpawnPoint.rotation;

        // Si el jugador ganó, marcar el NPC como derrotado
        if (playerWon && pendingCombatNPC != null)
            pendingCombatNPC.MarcarComoDerrotado();

        pendingCombatNPC = null;

        battleCanva.SetActive(false);

        yield return StartCoroutine(Fade(1f, 0f));

    }

    IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
    }
}