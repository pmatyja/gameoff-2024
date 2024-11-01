using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TweenAnimator
{
    [SerializeField]
    private List<TweenAnimation> queue = new();

    private int groupId;

    public TweenAnimator Next(float duration = 1.0f, bool pingPong = false, int loopCount = 1, Func<float, float> interpolationMethod = null, Action<float> onUpdate = null, Action onComplete = null)
    {
        this.queue.Add(new TweenAnimation(this.groupId++, pingPong, loopCount, duration, interpolationMethod ?? Easing.SmoothStep, onUpdate, onComplete));
        return this;
    }

    public TweenAnimator Next(Action onComplete)
    {
        this.queue.Add(new TweenAnimation(this.groupId++, onComplete: onComplete));
        return this;
    }

    public TweenAnimator Parallel(float duration = 1.0f, bool pingPong = false, int loopCount = 1, Func<float, float> interpolationMethod = null, Action<float> onUpdate = null, Action onComplete = null)
    {
        this.queue.Add(new TweenAnimation(this.groupId, pingPong, loopCount, duration, interpolationMethod ?? Easing.SmoothStep, onUpdate, onComplete));
        return this;
    }

    public TweenAnimator Parallel(Action onComplete)
    {
        this.queue.Add(new TweenAnimation(this.groupId, onComplete: onComplete));
        return this;
    }

    public void Update()
    {
        if (this.queue.Any())
        {
            var nextGroupId = this.queue.Min(x => x.GroupId);

            for (var i = 0; i < this.queue.Count;)
            {
                if (this.queue[i].GroupId == nextGroupId)
                {
                    this.queue[i].Update();

                    if (this.queue[i].IsDone)
                    {
                        this.queue.RemoveAt(i);
                        continue;
                    }
                }

                i++;
            }
        }
    }
}
