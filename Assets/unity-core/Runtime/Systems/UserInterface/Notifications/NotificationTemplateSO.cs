using UnityEngine;

[CreateAssetMenu(fileName = nameof(NotificationTemplateSO), menuName = "Lavgine/UI/Notification Template")]
public class NotificationTemplateSO : ScriptableObject
{
    [SerializeField]
    private Color color;
    public Color Color => this.color;

    [SerializeField]
    [Translation]
    private string title;
    public string Title => this.title;

    [SerializeField]
    private SoundEffectSO audioCude;
    public SoundEffectSO AudioCue => this.audioCude;

    [ContextMenu("Push")]
    private void PushNotification()
    {
        EventBus.Raise(this, new QueueNotificationEventParameters{ Event = this.name, Content = "Template Content Text" });
    }
}