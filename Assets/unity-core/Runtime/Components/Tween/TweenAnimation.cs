using System;
using UnityEngine;

[Serializable]
public class TweenAnimation
{
    [SerializeField]
    [Readonly]
    private int groupId;
    public int GroupId => this.groupId;

    [SerializeField]
    [Readonly]
    private bool isDone;
    public bool IsDone => this.isDone;

    [SerializeField]
    [Readonly]
    private bool pingPong;

    [SerializeField]
    [Readonly]
    private int loopCount;

    [SerializeField]
    [Readonly]
    private float progress;

    [SerializeField]
    [Readonly]
    private float duration;

    private readonly Func<float, float> interpolationMethod;
    private readonly Action<float> onUpdate;
    private readonly Action onComplete;

    public TweenAnimation(int groupId, bool pingPong = false, int loopCount = 1, float duration = 0.0f, Func<float, float> interpolationMethod = null, Action<float> onUpdate = null, Action onComplete = null)
    {
        this.groupId = groupId;
        this.pingPong = pingPong;
        this.loopCount = loopCount;
        this.duration = duration;
        this.interpolationMethod = interpolationMethod;
        this.onUpdate = onUpdate;
        this.onComplete = onComplete;
    }

    public void Update()
    {
        if (this.isDone)
        {
            return;
        }

        if (this.duration <= 0.0f)
        {
            this.onComplete?.Invoke();
            this.isDone = true;
            return;
        }

        this.progress = Tween.Interpolate
        (
            this.progress, 
            this.duration, 
            ref this.loopCount, 
            this.pingPong, 
            1.0f,
            value => this.onUpdate?.Invoke(this.interpolationMethod.Invoke(value)), 
            this.onComplete
        );

        if (this.progress >= 1.0f)
        {
            this.loopCount--;

            if (this.loopCount > 0)
            {
                this.progress = 0.0f;
                return;
            }

            this.isDone = true;
        }
    }
}
