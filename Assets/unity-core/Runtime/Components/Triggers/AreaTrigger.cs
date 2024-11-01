using UnityEngine;

public abstract class AreaTrigger : Trigger
{
    [SerializeReference]
    private NodeGraph OnEnter ;

    [SerializeReference]
    private NodeGraph OnExit;

    [SerializeField]
    private EventChannelSO eventChannel;

    [SerializeField]
    private Prompt prompt;

    [SerializeField]
    [Readonly]
    private bool inTriggerVolume;
    public bool InTriggerVolume => this.inTriggerVolume;

    [SerializeField]
    [Readonly]
    private bool canTrigger = true;

    [SerializeField]
    private bool repeatable;

    [SerializeField]
    private LayerMask layerMask;
    public LayerMask LayerMask => this.layerMask;

    [SerializeField]
    private bool showArea = true;

    [SerializeField]
    private Color areaColor = Color.cyan;

    public bool HasPrompt => this.prompt != null;

    protected abstract bool Overlaps();
    protected abstract void DrawGizmos();

    protected virtual bool CanTrigger()
    {
        if (this.IsEnabled == false)
        {
            return false;
        }

        if (this.InTriggerVolume == false)
        {
            return false;
        }

        if (this.repeatable == false)
        {
            if (this.Visited)
            {
                return false;
            }
        }

        if (this.IsEnterOnly())
        {
            if (this.canTrigger == false)
            {
                return false;
            }
        }

        return true;
    }

    protected override bool CanRepeat()
    {
        return this.repeatable;
    }

    protected virtual bool IsEnterOnly()
    {
        if (this.prompt != null)
        {
            return false;
        }

        return true;
    }

    protected override bool ShouldTrigger()
    {
        if (base.ShouldTrigger() == false)
        {
            return false;
        }

        if (this.inTriggerVolume == false)
        {
            return false;
        }

        if (this.IsEnterOnly())
        {
            if (this.canTrigger == false)
            {
                return false;
            }
        }

        if (this.prompt != null)
        {
            return InputManager.Pressed(this.prompt.Source);
        }

        this.canTrigger = false;
        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.OnEnter?.OnEnable();
        this.OnExit?.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.OnEnter?.OnDisable();
        this.OnExit?.OnDisable();
    }

    protected override void Update()
    {
        if (this.IsEnabled == false)
        {
            if (this.inTriggerVolume)
            {
                this.inTriggerVolume = false;
                this.OnExit?.Schedule();

                EventBus.Raise(this, this.eventChannel, new AreaTriggerEventParameters
                {
                    Source = this.gameObject,
                    State = AreaTriggerState.Exit
                });
            }

            this.prompt?.Hide();
            return;
        }

        if (this.Overlaps())
        {
            if (this.inTriggerVolume == false)
            {
                this.inTriggerVolume = true;
                this.OnEnter?.Schedule();

                EventBus.Raise(this, this.eventChannel, new AreaTriggerEventParameters
                {
                    Source = this.gameObject,
                    State = AreaTriggerState.Enter
                });
            }
        }
        else
        {
            if (this.inTriggerVolume)
            {
                this.inTriggerVolume = false;
                this.OnExit?.Schedule();

                EventBus.Raise(this, this.eventChannel, new AreaTriggerEventParameters
                {
                    Source = this.gameObject,
                    State = AreaTriggerState.Exit
                });

                this.prompt?.Hide();
            }

            this.canTrigger = true;
        }
        
        if (this.repeatable == false)
        {
            if (this.Visited)
            {
                this.prompt?.Hide();
                return;
            }
        }

        if (this.inTriggerVolume)
        {
            EventBus.Raise(this, this.eventChannel, new AreaTriggerEventParameters
            {
                Source = this.gameObject,
                State = AreaTriggerState.Stay
            });
        }

        if (this.CanTrigger())
        {
            this.prompt?.Show();
        }

        base.Update();
    }

    private void OnDrawGizmos()
    {
        if (this.showArea)
        {
            this.OnDrawGizmosSelected();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = this.areaColor;
        this.DrawGizmos();
    }

    private void OnValidate()
    {
        if (this.layerMask == 0)
        {
            this.layerMask = LayerMask.GetMask(LayerMask.LayerToName(3));
        }
    }
}