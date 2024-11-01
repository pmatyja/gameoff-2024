using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class QuestTracker : UiElement
{
    [SerializeField]
    [NotNull("File 'Shared/VTA_Gameplay_QuestsTracker_Quest.uxml' not found")]
    [Readonly]
    private VisualTreeAsset questTemplate;

    [SerializeField]
    [NotNull("File 'Shared/VTA_Gameplay_QuestsTracker_Objective.uxml' not found")]
    [Readonly]
    private VisualTreeAsset objectiveTemplate;

    private List<QuestSO> quests = new();
    private List<ObjectiveSO> objectives = new();

    [SerializeField]
    private bool alwaysVisible = true;

    [SerializeField]
    [Range(5, 15)]
    private int delayBeforeHidden = 10;

    [SerializeField]
    [Readonly]
    private float timeBeforeHidden = 0.0f;

    [SerializeField]
    [Range(3, 10)]
    private int delayBeforeRemoval = 5;

    public override bool IsVisible(GameMode mode)
    {
        return mode == GameMode.Gameplay || mode == GameMode.Settings;
    }

    public override void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        base.Enable(behaviour, root.Q<VisualElement>(nameof(QuestTracker)));

        this.Root.Clear();

        EventBus.AddListener<QuestStartEventParameters>(this.OnQuestStartEvent);
        EventBus.AddListener<QuestFailedEventParameters>(this.OnQuestFailedEvent);
        EventBus.AddListener<QuestSucceededEventParameters>(this.OnQuestSucceededEvent);

        EventBus.AddListener<ObjectiveActivateEventParameters>(this.OnObjectiveActivateEvent);
        EventBus.AddListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceededEvent);
        EventBus.AddListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailedEvent);
    }

    public override void Disable()
    {
        base.Disable();

        EventBus.RemoveListener<QuestStartEventParameters>(this.OnQuestStartEvent);
        EventBus.RemoveListener<QuestFailedEventParameters>(this.OnQuestFailedEvent);
        EventBus.RemoveListener<QuestSucceededEventParameters>(this.OnQuestSucceededEvent);

        EventBus.RemoveListener<ObjectiveActivateEventParameters>(this.OnObjectiveActivateEvent);
        EventBus.RemoveListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceededEvent);
        EventBus.RemoveListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailedEvent);

        this.quests.Clear();
        this.objectives.Clear();
    }

    public override void Update()
    {
        base.Update();

        if (this.alwaysVisible)
        {
            this.timeBeforeHidden = this.delayBeforeHidden;
        }

        this.timeBeforeHidden = Mathf.Max(0.0f, this.timeBeforeHidden - Time.deltaTime);
        this.Root.Fade(this.timeBeforeHidden == 0.0f ? 0.0f : 1.0f, 0.25f);

        foreach (var quest in this.quests)
        {
            if (this.Root.TryGet<VisualElement>(quest.Id.ToString(), out var questElement))
            {
                if (questElement.TryGet<Label>("Content", out var title))
                {
                    title.text = quest.ToString().Localize();
                }

                foreach (var objective in quest.Objectives)
                {
                    if (questElement.TryGet<VisualElement>(objective.Id.ToString(), out var objectiveElement))
                    {
                        if (objectiveElement.TryGet<Label>("Content", out var content))
                        {
                            content.text = objective.ToString().Localize();
                        }
                    }
                }
            }
        }
    }

    public override void OnValidate()
    {
        base.OnValidate();

        EditorOnly.LoadAsset("Assets/Resources/Shared/VTA_Gameplay_QuestsTracker_Quest.uxml", out this.questTemplate);
        EditorOnly.LoadAsset("Assets/Resources/Shared/VTA_Gameplay_QuestsTracker_Objective.uxml", out this.objectiveTemplate);
    }

    private void OnQuestStartEvent(object sender, QuestStartEventParameters parameters)
    {
        if (parameters.Quest == null)
        {
            return;
        }

        if (this.quests.Contains(parameters.Quest) == false)
        {
            this.timeBeforeHidden = this.delayBeforeHidden;
            this.quests.Add(parameters.Quest);
            this.Behaviour.StartCoroutine(this.AddQuest(parameters.Quest));
        }
    }

    private void OnQuestFailedEvent(object sender, QuestFailedEventParameters parameters)
    {
        if (parameters.Quest == null)
        {
            return;
        }

        this.timeBeforeHidden = this.delayBeforeHidden;
        this.Behaviour.StartCoroutine(this.OnRemove(parameters.Quest));
    }

    private void OnQuestSucceededEvent(object sender, QuestSucceededEventParameters parameters)
    {
        if (parameters.Quest == null)
        {
            return;
        }

        this.timeBeforeHidden = this.delayBeforeHidden;
        this.Behaviour.StartCoroutine(this.OnRemove(parameters.Quest));
    }

    private void OnObjectiveActivateEvent(object sender, ObjectiveActivateEventParameters parameters)
    {
        if (parameters.Objective == null)
        {
            return;
        }

        if (this.objectives.Contains(parameters.Objective) == false)
        {
            var quest = this.quests.FirstOrDefault(q => q.Objectives.Contains(parameters.Objective));
            if (quest == null)
            {
                Debug.LogWarning($"No quest active for objective '{parameters.Objective.Name}'");
                return;
            }

            if (quest.Status != ProgressStatus.InProgress)
            {
                Debug.LogWarning($"Invalid quest '{quest.Name}' status: '{quest.Status}'");
                return;
            }

            if (this.Root.TryGet<VisualElement>(quest.Id.ToString(), out var parent))
            {
                var objectiveElement = this.objectiveTemplate.Instantiate().Children().First();

                objectiveElement.name = parameters.Objective.Id.ToString();
                objectiveElement.Q<Label>("Content").text = parameters.Objective.ToString();

                parent.Add(objectiveElement);

                if (parameters.Objective.IsActive)
                {
                    this.timeBeforeHidden = this.delayBeforeHidden;
                    this.Behaviour.StartCoroutine(objectiveElement.FadeIn());
                }
            }
        }
    }

    private void OnObjectiveFailedEvent(object sender, ObjectiveFailedEventParameters parameters)
    {
        if (parameters.Objective == null)
        {
            return;
        }

        if (this.objectives.Contains(parameters.Objective))
        {
            this.timeBeforeHidden = this.delayBeforeHidden;
        }
    }

    private void OnObjectiveSucceededEvent(object sender, ObjectiveSucceededEventParameters parameters)
    {
        if (parameters.Objective == null)
        {
            return;
        }

        if (this.objectives.Contains(parameters.Objective))
        {
            this.timeBeforeHidden = this.delayBeforeHidden;
        }
    }

    private IEnumerator AddQuest(QuestSO quest)
    {
        var questElement = this.questTemplate.Instantiate();

        questElement.name = quest.Id.ToString();
        questElement.Q<Label>("Content").text = quest.ToString();

        this.Root.Add(questElement);

        yield return questElement.FadeIn();

        if (quest.Status == ProgressStatus.InProgress)
        {
            foreach (var objective in quest.Objectives.OrderBy(x => x.Order).Where(x => x.IsActive))
            {
                var objectiveElement = this.objectiveTemplate.Instantiate();

                objectiveElement.name = objective.Id.ToString();
                objectiveElement.Q<Label>("Content").text = objective.ToString();

                questElement.Add(objectiveElement);

                if (objective.IsActive)
                {
                    yield return objectiveElement.FadeIn();
                }
            }
        }
    }

    private IEnumerator OnRemove(QuestSO quest)
    {
        if (this.Root.TryGet<VisualElement>(quest.Id.ToString(), out var element))
        {
            yield return Wait.Seconds(this.delayBeforeRemoval);

            yield return element.FadeOut();

            element.RemoveFromHierarchy();
        }

        if (this.quests.Contains(quest))
        {
            this.quests.Remove(quest);
        }
    }
}
