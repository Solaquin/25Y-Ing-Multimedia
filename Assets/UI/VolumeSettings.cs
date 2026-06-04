
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider fxSlider;

    private void Start()
    {
        if (AudioManagerMenu.Instance == null)
        {
            Debug.LogError("No existe AudioManagerMenu en la escena");
            return;
        }

        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;

            musicSlider.value =
                AudioManagerMenu.Instance.GetMusicVolume();

            musicSlider.onValueChanged.AddListener(
                AudioManagerMenu.Instance.SetMusicVolume);
        }

        if (fxSlider != null)
        {
            fxSlider.minValue = 0f;
            fxSlider.maxValue = 1f;

            fxSlider.value =
                AudioManagerMenu.Instance.GetFXVolume();

            fxSlider.onValueChanged.AddListener(
                AudioManagerMenu.Instance.SetFXVolume);
        }
    }

    private void OnDestroy()
    {
        if (AudioManagerMenu.Instance == null)
            return;

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(
                AudioManagerMenu.Instance.SetMusicVolume);
        }

        if (fxSlider != null)
        {
            fxSlider.onValueChanged.RemoveListener(
                AudioManagerMenu.Instance.SetFXVolume);
        }
    }
}
