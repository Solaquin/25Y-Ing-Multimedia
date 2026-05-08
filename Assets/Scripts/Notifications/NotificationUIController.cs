using UnityEngine;
using System.Collections;

public class NotificationUIController : MonoBehaviour
{
    [SerializeField]
    private NotificationItemUI notificationPrefab;

    [SerializeField]
    private Transform container;

    public IEnumerator ShowNotification(NotificationData data)
    {
        NotificationItemUI item =
            Instantiate(notificationPrefab, container);

        item.Setup(data);

        yield return StartCoroutine(item.Play());
    }
}