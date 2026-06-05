using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientMusicManager : MonoBehaviour
{
    [Header("Playlist (Modo Libre)")]
    [Tooltip("Arrastra hasta 5 canciones. Las ranuras vacías se ignoran.")]
    public AudioClip[] playlist = new AudioClip[5];

    [Header("Espera entre canciones (segundos)")]
    public float minWaitTime = 60f;
    public float maxWaitTime = 180f;

    [Header("Fade por canción")]
    [Tooltip("Entrada suave al iniciar cada pista.")]
    public float trackFadeInDuration = 2.5f;

    [Tooltip("Salida suave antes de que termine la pista (independiente del combate).")]
    public float trackFadeOutDuration = 3f;

    [Header("Combate")]
    public float combatFadeOutDuration = 1.5f;

    [Header("Volumen")]
    [Range(0f, 1f)]
    public float baseVolume = 0.4f;

    private AudioSource audioSource;
    private Coroutine musicCycleCoroutine;
    private Coroutine combatFadeCoroutine;
    private Coroutine trackVolumeFadeCoroutine;
    private bool isFreeRoamActive;
    private bool isVolumeFadeActive;
    private int lastTrackIndex = -1;

    private float TargetVolume => baseVolume * AudioInteractivo.GlobalMusicVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartFreeRoamMusic();
    }

    private void Update()
    {
        if (isFreeRoamActive && !isVolumeFadeActive && combatFadeCoroutine == null)
            ApplyVolume();
    }

    private void OnValidate()
    {
        if (maxWaitTime < minWaitTime)
            maxWaitTime = minWaitTime;

        if (trackFadeOutDuration < 0f)
            trackFadeOutDuration = 0f;

        if (trackFadeInDuration < 0f)
            trackFadeInDuration = 0f;

        if (playlist == null || playlist.Length != 5)
            playlist = new AudioClip[5];
    }

    public void StartFreeRoamMusic()
    {
        if (isFreeRoamActive && musicCycleCoroutine != null)
            return;

        StopCombatFadeIfRunning();
        StopTrackVolumeFadeIfRunning();

        isFreeRoamActive = true;

        if (musicCycleCoroutine != null)
            StopCoroutine(musicCycleCoroutine);

        musicCycleCoroutine = StartCoroutine(MusicCycleRoutine());
    }

    public void StopMusicForCombat()
    {
        isFreeRoamActive = false;

        if (musicCycleCoroutine != null)
        {
            StopCoroutine(musicCycleCoroutine);
            musicCycleCoroutine = null;
        }

        StopTrackVolumeFadeIfRunning();
        StopCombatFadeIfRunning();

        if (audioSource.isPlaying)
            combatFadeCoroutine = StartCoroutine(CombatFadeOutAndStop());
    }

    private IEnumerator MusicCycleRoutine()
    {
        while (isFreeRoamActive)
        {
            int trackIndex = PickRandomTrackIndex();
            if (trackIndex < 0)
            {
                Debug.LogWarning("[AmbientMusicManager] No hay AudioClips asignados en la playlist.");
                yield break;
            }

            yield return PlayTrackWithFades(playlist[trackIndex]);

            if (!isFreeRoamActive)
                yield break;

            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator PlayTrackWithFades(AudioClip clip)
    {
        float clipLength = clip.length;
        float targetVolume = TargetVolume;
        float fadeOutStartTime = GetFadeOutStartTime(clipLength);

        audioSource.clip = clip;
        audioSource.time = 0f;
        audioSource.volume = 0f;
        audioSource.Play();

        Debug.Log(
            $"[AmbientMusicManager] Reproduciendo '{clip.name}' " +
            $"({clipLength:F1}s). Fade out a los {fadeOutStartTime:F1}s.");

        yield return RunTrackVolumeFade(0f, targetVolume, trackFadeInDuration);

        if (!isFreeRoamActive)
            yield break;

        while (isFreeRoamActive && audioSource.isPlaying && audioSource.time < fadeOutStartTime)
            yield return null;

        if (!isFreeRoamActive)
            yield break;

        if (audioSource.isPlaying)
            yield return RunTrackVolumeFade(audioSource.volume, 0f, trackFadeOutDuration);

        while (isFreeRoamActive && audioSource.isPlaying)
            yield return null;

        audioSource.Stop();
        audioSource.clip = null;
        isVolumeFadeActive = false;
    }

    private float GetFadeOutStartTime(float clipLength)
    {
        if (clipLength <= 0f)
            return 0f;

        float fadeOutStart = clipLength - trackFadeOutDuration;

        if (fadeOutStart <= trackFadeInDuration)
            fadeOutStart = clipLength * 0.65f;

        return Mathf.Clamp(fadeOutStart, 0f, clipLength);
    }

    private IEnumerator RunTrackVolumeFade(float from, float to, float duration)
    {
        StopTrackVolumeFadeIfRunning();

        trackVolumeFadeCoroutine = StartCoroutine(TrackVolumeFadeRoutine(from, to, duration));
        yield return trackVolumeFadeCoroutine;
        trackVolumeFadeCoroutine = null;
    }

    private IEnumerator TrackVolumeFadeRoutine(float from, float to, float duration)
    {
        isVolumeFadeActive = true;

        if (duration <= 0f)
        {
            audioSource.volume = to;
            isVolumeFadeActive = false;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            audioSource.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }

        audioSource.volume = to;
        isVolumeFadeActive = false;
    }

    private IEnumerator CombatFadeOutAndStop()
    {
        isVolumeFadeActive = true;

        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < combatFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / combatFadeOutDuration);
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = 0f;
        isVolumeFadeActive = false;
        combatFadeCoroutine = null;
    }

    private int PickRandomTrackIndex()
    {
        List<int> validIndices = GetValidTrackIndices();
        if (validIndices.Count == 0)
            return -1;

        if (validIndices.Count == 1)
        {
            lastTrackIndex = validIndices[0];
            return validIndices[0];
        }

        int pickedIndex;
        do
        {
            pickedIndex = validIndices[Random.Range(0, validIndices.Count)];
        }
        while (pickedIndex == lastTrackIndex);

        lastTrackIndex = pickedIndex;
        return pickedIndex;
    }

    private List<int> GetValidTrackIndices()
    {
        var validIndices = new List<int>();

        if (playlist == null)
            return validIndices;

        for (int i = 0; i < playlist.Length; i++)
        {
            if (playlist[i] != null)
                validIndices.Add(i);
        }

        return validIndices;
    }

    private void ApplyVolume()
    {
        audioSource.volume = TargetVolume;
    }

    private void StopTrackVolumeFadeIfRunning()
    {
        if (trackVolumeFadeCoroutine == null)
            return;

        StopCoroutine(trackVolumeFadeCoroutine);
        trackVolumeFadeCoroutine = null;
        isVolumeFadeActive = false;
    }

    private void StopCombatFadeIfRunning()
    {
        if (combatFadeCoroutine == null)
            return;

        StopCoroutine(combatFadeCoroutine);
        combatFadeCoroutine = null;
        isVolumeFadeActive = false;
    }

    [ContextMenu("Test: Start Free Roam Music")]
    private void TestStartFreeRoamMusic()
    {
        isFreeRoamActive = false;
        if (musicCycleCoroutine != null)
        {
            StopCoroutine(musicCycleCoroutine);
            musicCycleCoroutine = null;
        }

        StartFreeRoamMusic();
    }

    [ContextMenu("Test: Stop Music For Combat")]
    private void TestStopMusicForCombat()
    {
        StopMusicForCombat();
    }
}
