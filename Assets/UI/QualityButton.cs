using UnityEngine;

public class QualityButton : MonoBehaviour
{
    [Tooltip("0 = Low, 1 = Medium, 2 = High")]
    public int qualityLevel = 0;

    public void ApplyQuality()
    {
        QualitySettings.SetQualityLevel(qualityLevel, true);
        Debug.Log($"Calidad: {QualitySettings.names[qualityLevel]}");
    }
}