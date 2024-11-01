using System;
using UnityEngine;

[Serializable]
public class TweenCurve : Tween
{
    [SerializeField]
    protected AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public AnimationCurve Curve => this.curve;

    public TweenCurve(float duration = 1.0f, float target = 1.0f)
    {
        this.target = target;
        this.duration = duration;
        this.Method = this.curve.Evaluate;
    }
}
