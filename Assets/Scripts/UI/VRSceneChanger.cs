using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class VRSceneChanger : MonoBehaviour
{
    [Header("Escena a cargar")]
    public string sceneName;

    [Header("Canvas de carga")]
    public GameObject loadingCanvas;

    [Header("Slider")]
    public Slider slider;

    [Header("Color de progreso")]
    public Color fillColor = Color.green;

    [Header("Icono que gira")]
    public RectTransform loadingIcon; // Puede ser Image o UI

    [Header("Velocidad de giro (grados/seg)")]
    public float rotationSpeed = 180f;

    [Header("Duración del fade")]
    public float fadeDuration = 1f;

    [Header("Tiempo de animación del slider (segundos)")]
    public float sliderFillDuration = 2f;

    private float displayedProgress = 0f;

    public void LoadScene()
    {
        if (loadingCanvas != null)
            loadingCanvas.SetActive(true);

        StartCoroutine(LoadAsyncScene());
    }

    private IEnumerator LoadAsyncScene()
    {
        // Fade in del Canvas
        CanvasGroup cg = loadingCanvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = loadingCanvas.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        // Cargar escena en segundo plano
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Progreso objetivo
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Suavizar el slider
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime / sliderFillDuration);
            slider.value = displayedProgress;

            // Cambiar color del fill
            if (slider.fillRect != null)
            {
                Image fillImage = slider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = fillColor;
            }

            // Girar el icono
            if (loadingIcon != null)
                loadingIcon.Rotate(0, 0, -rotationSpeed * Time.deltaTime); // Gira en Z (UI Image)

            // Activar escena cuando está lista
            if (operation.progress >= 0.9f && displayedProgress >= 1f)
            {
                operation.allowSceneActivation = true;
                yield return new WaitForSeconds(0.5f);
            }
                



            yield return null;
        }
    }
}