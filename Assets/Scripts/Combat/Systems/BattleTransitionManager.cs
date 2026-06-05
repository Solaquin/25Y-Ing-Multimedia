using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

public class BattleTransitionManager : MonoBehaviour
{
    public static BattleTransitionManager Instance { get; private set; }

    [Header("Referencias")]
    public BattleSystem battleSystem;       // El GameObject del BattleSystem (desactivado al inicio)
    public Transform battleZoneSpawnPoint;  // Punto donde aparece el jugador al entrar al combate
    public Transform worldSpawnPoint;       // Punto fijo donde vuelve el jugador al salir del combate
    public XROrigin xrRig;               // Tu XR Origin / XR Rig

    public GameObject canchaLimite;

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

        canchaLimite.SetActive(true);

        // 2. Teletransportar XR Rig a la zona de combate
        float headHeight = xrRig.CameraInOriginSpaceHeight;

        Vector3 targetPosition = battleZoneSpawnPoint.position + Vector3.up * headHeight;

        xrRig.MoveCameraToWorldLocation(targetPosition);

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
    public IEnumerator EndBattleTransition(bool playerWon)
    {
        yield return StartCoroutine(TransitionToWorld(playerWon));
    }

    IEnumerator TransitionToWorld(bool playerWon)
    {
        int dineroGanado = 0;
        CartaSO cartaGanada = null;

        yield return StartCoroutine(Fade(0f, 1f));

        canchaLimite.SetActive(false);

        if (battleSystem != null)
        {
            battleSystem.CleanupBattle();
        }

        // Desactivar BattleSystem
        battleSystem.gameObject.SetActive(false);

        // Volver al mundo (posición fija asignada en el Inspector)
        float headHeight = xrRig.CameraInOriginSpaceHeight;

        Vector3 targetPosition = worldSpawnPoint.position + Vector3.up * headHeight;

        xrRig.MoveCameraToWorldLocation(targetPosition);

        // Si el jugador ganó, marcar el NPC como derrotado
        if (playerWon && pendingCombatNPC != null)
        {
            dineroGanado = pendingCombatNPC.DineroOtorgado;
            cartaGanada = pendingCombatNPC.CartaRecompensa;

            pendingCombatNPC.MarcarComoDerrotado();
            pendingCombatNPC.EntregarRecompensas();
        }

        pendingCombatNPC = null;

        battleCanva.SetActive(false);

        yield return StartCoroutine(Fade(1f, 0f));

        if (playerWon)
        {
            if (dineroGanado > 0)
                NotificationManager.Send($"¡Has ganado {dineroGanado} monedas!");

            if (cartaGanada != null)
                NotificationManager.Send($"¡Has obtenido la carta '{cartaGanada.nombreCarta}'!");
        }
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