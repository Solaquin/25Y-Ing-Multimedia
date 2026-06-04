using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider fxSlider;

    void Start()
    {
        // Inicializa con el volumen actual del AudioManager
        if (musicSlider != null)
        {
            musicSlider.value = AudioManagerMenu.Instance.GetMusicVolume();
            musicSlider.onValueChanged.AddListener(AudioManagerMenu.Instance.SetMusicVolume);
        }

        if (fxSlider != null)
        {
            fxSlider.value = AudioManagerMenu.Instance.GetFXVolume();
            fxSlider.onValueChanged.AddListener(AudioManagerMenu.Instance.SetFXVolume);
        }
    }

    void OnDestroy()
    {
        musicSlider?.onValueChanged.RemoveListener(AudioManagerMenu.Instance.SetMusicVolume);
        fxSlider?.onValueChanged.RemoveListener(AudioManagerMenu.Instance.SetFXVolume);
    }
}