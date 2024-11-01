using UnityEngine;

public class TimerTrigger : Trigger
{
    [SerializeField]
    [Range(0.001f, 360.0f)]
    private float delay = 5.0f;

    [SerializeField]
    [Readonly]
    private float currrentTime;

    public override void Reset()
    {
        base.Reset();
        this.currrentTime = 0.0f;
    }

    protected override bool ShouldTrigger()
    {
        return base.ShouldTrigger() && this.currrentTime >= this.delay;
    }

    protected override void Update()
    {
        base.Update();

        this.currrentTime = Mathf.Min(this.currrentTime + Time.deltaTime, this.delay);
    }
}