using System;
using UnityEngine;

public abstract class ObjectiveSO : ScriptableObject
{
    public int Id => this.GetInstanceID();
    public string ValueStoreId => $"{this.Id}.{this.Name}";

    [Range(0, 20)]
    public int Order;

    [Translation]
    public string Name;

    [Translation]
    public string Description;

    public ProgressStatus Status
    {
        get => ValueStore.Get($"{this.ValueStoreId}.{nameof(this.Status)}", ProgressStatus.InProgress);
        protected set => ValueStore.Set($"{this.ValueStoreId}.{nameof(this.Status)}", value);
    }

    [SerializeField]
    public bool startActive = true;
    public bool IsActive
    { 
        get => this.startActive || ValueStore.Get($"{this.ValueStoreId}.{nameof(this.IsActive)}", false); 
        set => ValueStore.Set($"{this.ValueStoreId}.{nameof(this.IsActive)}", value);
    }

    public bool IsOptional;

    [SerializeReference]
    public NodeGraph OnSuccess;

    [SerializeReference]
    public NodeGraph OnFail;

    public void SetSuccessState()
    {
        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        this.Status = ProgressStatus.Successed;
        this.OnSuccess?.Schedule();
    }

    public void SetFailedState()
    {
        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        this.Status = ProgressStatus.Failed;
        this.OnFail?.Schedule();
    }

    private void OnEnable()
    {
        EventBus.AddListener<ObjectiveAdvanceEventParameters>(this.OnObjectiveAdvanceEvent);
        EventBus.AddListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceededEvent);
        EventBus.AddListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailedEvent);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<ObjectiveActivateEventParameters>(this.ObjectiveActivateEvent);
        EventBus.RemoveListener<ObjectiveAdvanceEventParameters>(this.OnObjectiveAdvanceEvent);
        EventBus.RemoveListener<ObjectiveSucceededEventParameters>(this.OnObjectiveSucceededEvent);
        EventBus.RemoveListener<ObjectiveFailedEventParameters>(this.OnObjectiveFailedEvent);
    }

    protected virtual void ObjectiveActivateEvent(object sender, ObjectiveActivateEventParameters parameters)
    {
        if (parameters.Objective != this)
        {
            return;
        }

        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        this.IsActive = true;
    }

    protected abstract void OnObjectiveAdvanceEvent(object sender, ObjectiveAdvanceEventParameters parameters);

    protected virtual void OnObjectiveSucceededEvent(object sender, ObjectiveSucceededEventParameters parameters)
    {
        if (parameters.Objective != this)
        {
            return;
        }

        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        this.SetSuccessState();
    }

    protected virtual void OnObjectiveFailedEvent(object sender, ObjectiveFailedEventParameters parameters)
    {
        if (parameters.Objective != this)
        {
            return;
        }

        this.SetFailedState();
    }

    protected virtual string GetName()
    {
        return this.Name;
    }

    public override string ToString()
    {
        var content = this.GetName();

        if (this.IsOptional)
        {
            content += " (Optional)";
        }

        if (this.Status == ProgressStatus.Successed)
        {
            content = $"\ue834 <s>{content}</s>";
        }
        else if (this.Status == ProgressStatus.Failed)
        {
            content = $"\uf230 <s>{content}</s> [Failed]";
        }
        else
        {
            content = $"\ue835 {content}";
        }

        return content;
    }

    [ContextMenu("Reset Status (debug)")]
    private void RaiseReset()
    {
        this.Status = ProgressStatus.InProgress;
    }

    [ContextMenu("Events/Activate")]
    private void RaiseActivate()
    {
        EventBus.Raise(this, new ObjectiveActivateEventParameters { Objective = this });
    }

    [ContextMenu("Events/Advance")]
    private void RaiseAdvance()
    {
        EventBus.Raise(this, new ObjectiveAdvanceEventParameters { Objective = this });
    }

    [ContextMenu("Events/Fail")]
    private void RaiseFail()
    {
        EventBus.Raise(this, new ObjectiveFailedEventParameters { Objective = this });
    }

    [ContextMenu("Events/Succede")]
    private void RaiseSuccede()
    {
        EventBus.Raise(this, new ObjectiveSucceededEventParameters { Objective = this });
    }
}