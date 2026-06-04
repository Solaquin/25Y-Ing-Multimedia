public class NotificationData
{
    public NotificationSO template;

    public string customBody;

    public NotificationData(NotificationSO template)
    {
        this.template = template;

        customBody = template.defaultBody;
    }
}