using UnityEngine;

[CreateAssetMenu(menuName = "Notifications/Notification")]
public class NotificationSO : ScriptableObject
{
    public string id;

    [TextArea]
    public string defaultBody;

    public Sprite icon;

    public float duration = 5f;
}