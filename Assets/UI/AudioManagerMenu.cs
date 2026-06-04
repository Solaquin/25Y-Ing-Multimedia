using UnityEngine;

public class AudioManagerMenu : MonoBehaviour
{
    public static AudioManagerMenu Instance;

    [Header("Música")]
    public AudioSource[] musicSources;

    [Header("FX")]
    public AudioSource[] fxSources;

    private float musicVolume = 1f;
    private float fxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        foreach (AudioSource src in musicSources)
        {
            if (src == null) continue;
            src.volume = value;
            Debug.Log($"Música: {src.gameObject.name} volumen = {src.volume}");

            AudioInteractivo ai = src.GetComponent<AudioInteractivo>();
            if (ai != null) ai.volumen = value;
        }
    }

    public void SetFXVolume(float value)
    {
        fxVolume = value;
        foreach (AudioSource src in fxSources)
        {
            if (src == null) continue;
            src.volume = value;

            AudioInteractivo ai = src.GetComponent<AudioInteractivo>();
            if (ai != null) ai.volumen = value;
        }
    }

    public float GetMusicVolume() => musicVolume;
    public float GetFXVolume() => fxVolume;
}