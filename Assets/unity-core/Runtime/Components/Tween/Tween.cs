using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Tween
{
    public bool PingPong;
    public float Value => this.Method.Invoke(this.PingPong ? Tween.Bounce(this.progress) : this.progress);

    [SerializeField]
    public Func<float, float> Method = Easing.SmoothStep;

    [SerializeField]
    [Range(1, int.MaxValue / 10)]
    protected int loopCount = 1;
    public int LoopCount
    {
        get => this.loopCount;
        set => this.loopCount = Mathf.Clamp(value, 1, int.MaxValue / 10);
    }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float target = 1.0f;
    public float Target
    {
        get => this.target;
        set => this.target = Mathf.Clamp01(value);
    }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float progress;
    public float Progress
    {
        get => this.progress;
        set => this.progress = Mathf.Clamp01(value);
    }

    [SerializeField]
    [Range(0.0f, 60.0f)]
    protected float duration = 1.0f;
    public float Duration
    {
        get => this.duration;
        set => this.duration = Mathf.Clamp(value, 0.0f, 60.0f);
    }

    public Tween(float duration = 1.0f, float target = 1.0f)
    {
        this.target = target;
        this.duration = duration;
    }

    public static float Once(float progress, float duration = 1.0f, float target = 1.0f)
    {
        return Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
    }

    public static IEnumerator Once(float duration = 1.0f, float target = 1.0f, Action<float> onUpdate = null)
    {
        var progress = 1.0f - target;

        while (progress != target)
        {
            progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
            onUpdate?.Invoke(progress);

            yield return null;
        }
    }

    public static IEnumerator Once(float progress, float duration = 1.0f, float target = 1.0f, Action<float> onUpdate = null)
    {
        while (progress != target)
        {
            progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
            onUpdate?.Invoke(progress);

            yield return null;
        }
    }

    public static IEnumerator BounceOnce(float duration = 1.0f, float target = 1.0f, Action<float> onUpdate = null)
    {
        var progress = 1.0f - target;

        while (progress != target)
        {
            progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
            onUpdate?.Invoke(Tween.Bounce(progress));

            yield return null;
        }
    }

    public static IEnumerator BounceOnce(float progress, float duration = 1.0f, float target = 1.0f, Action<float> onUpdate = null)
    {
        while (progress != target)
        {
            progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
            onUpdate?.Invoke(Tween.Bounce(progress));

            yield return null;
        }
    }

    public static bool Repeat(ref float progress, ref int loopCount, float duration = 1.0f, float target = 1.0f)
    {
        if (loopCount < 1)
        {
            return false;
        }

        progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);

        if (progress == target)
        {
            loopCount--;
            progress = target - 1.0f;
        }

        return true;
    }

    public static IEnumerator Repeat(float duration = 1.0f, int loopCount = 1, float target = 1.0f, Action<float> onUpdate = null)
    {
        var progress = 1.0f - target;

        while (progress != target)
        {
            if (loopCount < 1)
            {
                yield break;
            }

            progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
            onUpdate?.Invoke(progress);

            if (progress == target)
            {
                loopCount--;
                progress = target - 1.0f;
            }

            yield return null;
        }
    }

    public static IEnumerator BounceRepeat(float duration = 1.0f, int loopCount = 1, float target = 1.0f, Action<float> onUpdate = null)
    {
        var progress = 1.0f - target;

        while (progress != target)
        {
            if (loopCount < 1)
            {
                yield break;
            }

            progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
            onUpdate?.Invoke(Tween.Bounce(progress));

            if (progress == target)
            {
                loopCount--;
                progress = target - 1.0f;
            }

            yield return null;
        }
    }

    public static float Bounce(float progress)
    {
        var t = progress;

        t = 2.0f * t;

        if (progress >= 0.5f)
        {
            return 1.0f - (t - 1.0f);
        }

        return t;
    }

    public static float Interpolate(float progress, float duration, bool pingPong = false, float target = 1.0f, Action<float> onUpdate = null, Action onCompleate = null)
    {
        var t = progress;

        if (pingPong)
        {
            t = 2.0f * t;

            if (progress >= 0.5f)
            {
                t = 1.0f - (t - 1.0f);
            }
        }

        onUpdate?.Invoke(t);

        if (progress == target)
        {
            onCompleate?.Invoke();
        }

        return Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
    }
    
    public static float Interpolate(float progress, float duration, ref int loopCount, bool pingPong = false, float target = 1.0f, Action<float> onUpdate = null, Action onCompleate = null)
    {
        if (loopCount > 0)
        {
            progress = Tween.Interpolate(progress, duration, pingPong, target, onUpdate, onCompleate);

            if (progress == target)
            {
                if (loopCount > 1)
                {
                    loopCount--;
                    progress = Mathf.Clamp01(Mathf.Abs(1.0f - progress));
                }
            }
        }

        return progress;
    }

    public static void Interpolate(Tween tween, Action<float> onUpdate = null, Action onCompleate = null)
    {
        tween.progress = Tween.Interpolate(tween.progress, tween.duration, ref tween.loopCount, tween.PingPong, tween.target, onUpdate, onCompleate);
    }
    
    public virtual void Reset()
    {
        this.progress = 0.0f;
    }

    public virtual void Update()
    {
        Tween.Interpolate(this);
    }
}
