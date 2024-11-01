using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
    [SerializeField]
    public bool IsEnabled = true;

    [SerializeField]
    [Readonly]
    private bool visited;
    public bool Visited => this.visited;

    [SerializeReference]
    private NodeGraph OnTriggered;

    protected virtual void OnEnable()
    {
        this.OnTriggered?.OnEnable();
    }

    protected virtual void OnDisable()
    {
        this.OnTriggered?.OnDisable();
    }

    public virtual void Reset()
    {
        this.visited = false;
    }

    public void InvokeTrigger()
    {
        if (this.visited)
        {
            if (this.CanRepeat() == false)
            {
                return;
            }
        }

        this.OnTriggered?.Schedule();

        EventBus.Raise(this, new TriggerEventParameters
        {
            Source = this
        });
        
        this.visited = true;
    }

    protected virtual bool CanRepeat()
    {
        return false;
    }

    protected virtual bool ShouldTrigger()
    {
        if (this.IsEnabled == false)
        {
            return false;
        }

        return true;
    }

    protected virtual void Update()
    {
        if (this.ShouldTrigger())
        {
            this.InvokeTrigger();
        }
    }
}