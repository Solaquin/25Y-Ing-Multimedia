using UnityEngine;

public class AudioManagerMenu : MonoBehaviour
{
    public static AudioManagerMenu Instance;

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
        AudioInteractivo.GlobalMusicVolume = value;

        AudioManager.RefreshVolumes();

        Debug.Log("Volumen Música = " + value);
    }

    public void SetFXVolume(float value)
    {
        AudioInteractivo.GlobalFXVolume = value;

        Debug.Log("Volumen FX = " + value);
    }

    public float GetMusicVolume()
    {
        return AudioInteractivo.GlobalMusicVolume;
    }

    public float GetFXVolume()
    {
        return AudioInteractivo.GlobalFXVolume;
    }
}
