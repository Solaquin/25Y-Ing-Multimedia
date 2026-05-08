using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Collections;

public class NotificationItemUI : MonoBehaviour
{
    [SerializeField]
    private Image icon;

    [SerializeField]
    private TMP_Text body;

    [SerializeField]
    private CanvasGroup canvasGroup;

    private NotificationData currentData;

    public void Setup(NotificationData data)
    {
        currentData = data;


        body.text = string.IsNullOrEmpty(data.customBody)
            ? data.template.defaultBody
            : data.customBody;

        icon.sprite = data.template.icon;
    }

    public IEnumerator Play()
    {
        yield return FadeIn();

        yield return new WaitForSeconds(
            currentData.template.duration
        );

        yield return FadeOut();

        Destroy(gameObject);
    }

    private IEnumerator FadeIn()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 4f;

            canvasGroup.alpha = t;

            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float t = 1;

        while (t > 0)
        {
            t -= Time.deltaTime * 4f;

            canvasGroup.alpha = t;

            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}