using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class NotificationSystem : UiElement
{
    private ConcurrentQueue<Notification> queue = new();

    [SerializeField]
    [Readonly]
    private List<NotificationTemplateSO> templates = new();

    private struct Notification
    {
        public NotificationTemplateSO Template;
        public string Content;
    }

    public override bool IsVisible(GameMode mode)
    {
        return mode == GameMode.Gameplay || mode == GameMode.Settings;
    }

    public override void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        base.Enable(behaviour, root.Q<VisualElement>(nameof(NotificationSystem)));

        EventBus.AddListener<QueueNotificationEventParameters>(this.OnNotification);

        EventBus.AddListener<QuestStartEventParameters>(this.OnQuestStart);
        EventBus.AddListener<QuestSucceededEventParameters>(this.OnQuestSucceeded);
        EventBus.AddListener<QuestFailedEventParameters>(this.OnQuestFailed);

        EventBus.AddListener<ObjectiveActivateEventParameters>(this.ObjectiveActivate);
        EventBus.AddListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceeded);
        EventBus.AddListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailed);
    }

    public override void Disable()
    {
        base.Disable();

        EventBus.RemoveListener<QueueNotificationEventParameters>(this.OnNotification);

        EventBus.RemoveListener<QuestStartEventParameters>(this.OnQuestStart);
        EventBus.RemoveListener<QuestSucceededEventParameters>(this.OnQuestSucceeded);
        EventBus.RemoveListener<QuestFailedEventParameters>(this.OnQuestFailed);

        EventBus.RemoveListener<ObjectiveActivateEventParameters>(this.ObjectiveActivate);
        EventBus.RemoveListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceeded);
        EventBus.RemoveListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailed);

        this.queue.Clear();
    }

    public override void OnValidate()
    {
        base.OnValidate();

        EditorOnly.LoadAsset<NotificationTemplateSO>("Assets/Resources/Shared/", "*.asset", this.templates, SearchOption.AllDirectories);
    }

    public override IEnumerator Animate()
    {
        this.Root.SetOpacity(0.0f);

        var title   = this.Root.Q<Label>("Title");
        var content = this.Root.Q<Label>("Content");

        while (this.Behaviour.isActiveAndEnabled)
        {
            if (GameManager.Mode == GameMode.Gameplay)
            {
                lock (this.queue)
                {
                    if (this.queue.TryPeek(out var notification))
                    {
                        this.Root.SetOpacity(0.0f);

                        if (title != null)
                        {
                            title.text = notification.Template.Title;
                        }

                        if (content != null)
                        {
                            content.text = notification.Content;
                            content.style.color = notification.Template.Color;
                        }

                        this.Root.Localize();

                        yield return this.Root.FadeIn();

                        notification.Template.AudioCue?.Play();

                        yield return Wait.Seconds(UITheme.GetTextDisplayTime(notification.Content, 5.0f));

                        yield return this.Root.FadeOut();

                        yield return Wait.Seconds(0.1f);

                        this.queue.TryDequeue(out _);
                    }
                }
            }

            yield return base.Animate();
        }
    }

    private void QueueNotification(string eventName, string content)
    {
        var template = this.templates.FirstOrDefault(x => string.Equals(x.name, eventName, StringComparison.OrdinalIgnoreCase));
        if (template == null)
        {
            Debug.LogWarning($"UI Event Template '{eventName}' is missing");
            return;
        }

        lock (this.queue)
        {
            this.queue.Enqueue(new Notification
            {
                Template = template,
                Content = content
            });
        }
    }

    private void OnNotification(object sender, QueueNotificationEventParameters parameters)
    {
        this.QueueNotification(parameters.Event, parameters.Content);
    }

    private string GetEventName<T>()
    {
        return $"NOT_{nameof(T)}";
    }

    private void OnQuestStart(object sender, LocationDiscoveredEventParameters parameters)
    {
        this.QueueNotification(this.GetEventName<LocationDiscoveredEventParameters>(), parameters.Location);
    }

    private void OnQuestStart(object sender, QuestStartEventParameters parameters)
    {
        this.QueueNotification(this.GetEventName<QuestStartEventParameters>(), parameters.Quest.Name);
    }

    private void OnQuestSucceeded(object sender, QuestSucceededEventParameters parameters)
    {
        this.QueueNotification(this.GetEventName<QuestSucceededEventParameters>(), parameters.Quest.Name);
    }

    private void OnQuestFailed(object sender, QuestFailedEventParameters parameters)
    {
        this.QueueNotification(this.GetEventName<QuestFailedEventParameters>(), parameters.Quest.Name);
    }

    private void ObjectiveActivate(object sender, ObjectiveActivateEventParameters parameters)
    {
        this.QueueNotification(this.GetEventName<ObjectiveActivateEventParameters>(), parameters.Objective.Name);
    }

    private void OnObjectiveSucceeded(object sender, ObjectiveSucceededEventParameters parameters)
    {
        this.QueueNotification(this.GetEventName<ObjectiveSucceededEventParameters>(), parameters.Objective.Name);
    }

    private void OnObjectiveFailed(object sender, ObjectiveFailedEventParameters parameters)
    {
        this.QueueNotification(this.GetEventName<ObjectiveFailedEventParameters>(), parameters.Objective.Name);
    }
}