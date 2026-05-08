using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [SerializeField]
    private NotificationUIController uiController;

    private Queue<NotificationData> queue =
        new Queue<NotificationData>();

    private bool isShowing;

    private void Awake()
    {
        Instance = this;
    }

    public static void Send(NotificationSO notification)
    {
        if (Instance == null)
            return;

        Instance.Enqueue(new NotificationData(notification));
    }

    public static void Send(NotificationData data)
    {
        if (Instance == null)
            return;

        Instance.Enqueue(data);
    }

    private void Enqueue(NotificationData data)
    {
        queue.Enqueue(data);

        if (!isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (queue.Count > 0)
        {
            NotificationData data = queue.Dequeue();

            yield return StartCoroutine(
                uiController.ShowNotification(data)
            );
        }

        isShowing = false;
    }
}