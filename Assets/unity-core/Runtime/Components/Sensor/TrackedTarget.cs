using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class TrackedTarget
{
    public readonly DetectionTarget Target;
    public bool Detected;
    public float DetectionState;

    public TrackedTarget(DetectionTarget target)
    {
        this.Target = target;
    }

    public void Calm(float calmRate)
    {
        this.Detected = false;
        this.DetectionState = Mathf.Max(0.0f, this.DetectionState - calmRate * Time.deltaTime);

        if (this.DetectionState > 0.0f)
        {
            this.Target.SetDetectionState(this.DetectionState);
        }
    }

    public void Detect(float detectionRate)
    {
        if (this.Detected)
        {
            return;
        }

        this.Detected = true;
        this.DetectionState = Mathf.Clamp01(this.DetectionState + detectionRate * Time.deltaTime);

        this.Target.SetDetectionState(this.DetectionState);
    }
}