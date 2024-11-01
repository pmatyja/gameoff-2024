using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(QuestSO), menuName = "Lavgine/Database.Questing/Quest")]
public class QuestSO : ScriptableObject
{
    public int Id => this.GetInstanceID();

    public string ValueStoreId => $"{this.Id}.{this.Name}";

    [Translation]
    public string Name;

    [Translation]
    public string Description;

    public ProgressStatus Status
    {
        get => ValueStore.Get($"{this.ValueStoreId}.{nameof(this.Status)}", ProgressStatus.InProgress);
        protected set => ValueStore.Set($"{this.ValueStoreId}.{nameof(this.Status)}", value);
    }

    public bool FinishOnLastObjective;

    public List<ObjectiveSO> Objectives = new();

    [SerializeReference]
    public NodeGraph OnSuccess;

    [SerializeReference]
    public NodeGraph OnFail;

    private void OnEnable()
    {
        EventBus.AddListener<QuestSucceededEventParameters>(this.OnQuestQuestSucceededEvent);
        EventBus.AddListener<QuestFailedEventParameters>(this.OnQuestFailedEvent);
        EventBus.AddListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceededEvent);
        EventBus.AddListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailedEvent);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<QuestSucceededEventParameters>(this.OnQuestQuestSucceededEvent);
        EventBus.RemoveListener<QuestFailedEventParameters>(this.OnQuestFailedEvent);
        EventBus.RemoveListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceededEvent);
        EventBus.RemoveListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailedEvent);
    }

    private void Update()
    {
        var nonOptional = this.Objectives.Where(x => x.IsOptional == false && x.IsActive);

        // if any non-optional objective fails then fail the quest

        if (nonOptional.Any(x => x.Status == ProgressStatus.Failed))
        {
            EventBus.Raise(this, new QuestFailedEventParameters
            {
                Quest = this
            });

            return;
        }

        // all objectives are OK
        if (FinishOnLastObjective)
        {
            if (nonOptional.All(x => x.Status == ProgressStatus.Successed))
            {
                EventBus.Raise(this, new QuestSucceededEventParameters
                {
                    Quest = this
                });
            }
        }
    }

    protected virtual void OnQuestQuestSucceededEvent(object sender, QuestSucceededEventParameters parameters)
    {
        if (parameters.Quest != this)
        {
            return;
        }

        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        this.Status = ProgressStatus.Successed;

        foreach (var objective in this.Objectives.Where(x => x.IsActive && x.Status == ProgressStatus.InProgress))
        {
            if (parameters.FailRemainingObjectives)
            {
                EventBus.Raise(this, new ObjectiveFailedEventParameters
                {
                    Objective = objective
                });
            }
            else
            {
                EventBus.Raise(this, new ObjectiveSucceededEventParameters
                {
                    Objective = objective
                });
            }
        }

        this.OnSuccess?.Schedule();
    }

    protected virtual void OnQuestFailedEvent(object sender, QuestFailedEventParameters parameters)
    {
        if (parameters.Quest != this)
        {
            return;
        }

        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        this.Status = ProgressStatus.Failed;

        foreach (var objective in this.Objectives.Where(x => x.IsActive && x.Status == ProgressStatus.InProgress))
        {
            EventBus.Raise(this, new ObjectiveFailedEventParameters
            {
                Objective = objective
            });
        }

        this.OnFail?.Schedule();
    }

    protected virtual void OnObjectiveSucceededEvent(object sender, ObjectiveSucceededEventParameters parameters)
    {
        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        if (this.Objectives.Contains(parameters.Objective) == false)
        {
            return;
        }

        parameters.Objective.SetSuccessState();
        
        this.Update();
    }

    protected virtual void OnObjectiveFailedEvent(object sender, ObjectiveFailedEventParameters parameters)
    {
        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        if (this.Objectives.Contains(parameters.Objective) == false)
        {
            return;
        }

        parameters.Objective.SetFailedState();

        this.Update();
    }

    public override string ToString()
    {
        if (this.Status == ProgressStatus.InProgress)
        {
            return $"\ue837 {this.Name}";
        }
        
        if (this.Status == ProgressStatus.Successed)
        {
            return $"\ue837 <s>{this.Name}</s>";
        }
        
        if (this.Status == ProgressStatus.Failed)
        {
            return $"\ue837 <s>{this.Name}</s> [Failed]";
        }

        return this.Name;
    }

    [ContextMenu("Reset Status (debug)")]
    private void RaiseReset()
    {
        this.Status = ProgressStatus.InProgress;
    }

    [ContextMenu("Events/Start")]
    private void RaiseStart()
    {
        EventBus.Raise(this, new QuestStartEventParameters { Quest = this });
    }

    [ContextMenu("Events/Fail")]
    private void RaiseFail()
    {
        EventBus.Raise(this, new QuestFailedEventParameters { Quest = this });
    }

    [ContextMenu("Events/Succede")]
    private void RaiseSuccede()
    {
        EventBus.Raise(this, new QuestSucceededEventParameters { Quest = this });
    }
}
