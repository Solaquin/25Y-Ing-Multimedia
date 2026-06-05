using UnityEngine;

/// <summary>
/// Conecta el AmbientMusicManager con los eventos de combate existentes.
/// Solo afecta audio; no modifica la lógica de batalla.
/// </summary>
public class AmbientMusicBattleListener : MonoBehaviour
{
    [SerializeField] private AmbientMusicManager ambientMusicManager;

    private void Awake()
    {
        if (ambientMusicManager == null)
            ambientMusicManager = FindFirstObjectByType<AmbientMusicManager>();
    }

    private void OnEnable()
    {
        BattleEvents.OnBattleIntroStarted += HandleBattleStarted;
        BattleEvents.OnBattleEnded += HandleBattleEnded;
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleIntroStarted -= HandleBattleStarted;
        BattleEvents.OnBattleEnded -= HandleBattleEnded;
    }

    private void HandleBattleStarted()
    {
        if (ambientMusicManager != null)
            ambientMusicManager.StopMusicForCombat();
    }

    private void HandleBattleEnded()
    {
        if (ambientMusicManager != null)
            ambientMusicManager.StartFreeRoamMusic();
    }
}
