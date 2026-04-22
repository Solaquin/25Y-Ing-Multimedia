using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float duracion = 2f;

    void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float tiempo = 0f;
        Color color = fadeImage.color;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempo / duracion);

            fadeImage.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0f);

        // Opcional: desactivar el panel al terminar
        fadeImage.gameObject.SetActive(false);
    }
}