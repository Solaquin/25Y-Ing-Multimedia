using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarUI : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;

    private float currentFill = 1f;
    private float targetFill = 1f;

    [Header("Animación")]
    public float fillSpeed = 3f;
    public float appearSpeed = 5f;

    private bool appearing = true;

    // 🔥 NUEVO: guardar escala real del prefab
    private Vector3 targetScale;

    void Start()
    {
        // Guarda la escala original (ej: 0.0006)
        targetScale = transform.localScale;

        // Empieza invisible
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        // 🔥 animación de aparición (usa escala real, no Vector3.one)
        if (appearing)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * appearSpeed
            );

            if (Vector3.Distance(transform.localScale, targetScale) < 0.00001f)
            {
                transform.localScale = targetScale;
                appearing = false;
            }
        }

        // 🔥 animación de vida (suave)
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * fillSpeed);
        fillImage.fillAmount = currentFill;

        // color dinámico
        fillImage.color = Color.Lerp(Color.red, Color.green, currentFill);
    }

    public void Setup(string name, int current, int max)
    {
        nameText.text = name;
        UpdateHP(current, max, true);
    }

    public void UpdateHP(int current, int max, bool instant = false)
    {
        float value = (float)current / max;

        targetFill = value;

        if (instant)
        {
            currentFill = value;
            fillImage.fillAmount = value;
        }

        hpText.text = current + " / " + max;
    }
}