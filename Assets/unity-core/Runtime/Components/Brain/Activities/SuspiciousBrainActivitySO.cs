using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = nameof(SuspiciousBrainActivitySO), menuName = "Lavgine/AI/Suspicious")]
public class SuspiciousBrainActivitySO : BrainActivitySO
{
    [SerializeField]
    [Range(0.0f, 1f)]
    private float minDetectionLevel = 0.1f;

    [SerializeField]
    [Range(0.0f, 1f)]
    private float chaseDetectionRate = 0.5f;

    public override bool CanActivate(BrainBehvaiour brain)
    {
        if (brain.TryGetComponent<IPlatformMover>(out var _) == false)
        {
            return false;
        }

        if (brain.TryGetComponent<FieldOfViewSensor2D>(out var view))
        {
            return true;
        }

        if (brain.TryGetComponent<AudioSensor2D>(out var sensor) == false)
        {
            return true;
        }

        return false;
    }

    public override float GetPriority(BrainBehvaiour brain)
    {
        if (this.FindBestTarget(brain, out var target))
        {
            if (target.DetectionState > this.minDetectionLevel)
            {
                return target.DetectionState;
            }
        }

        return 0.0f;
    }

    public override IEnumerator OnUpdate(BrainBehvaiour brain)
    {
        if (brain.TryGetComponent<IPlatformMover>(out var mover) == false)
        {
            yield break;
        }

        while (brain.IsRunningActivity(this))
        {
            if (this.FindBestTarget(brain, out var target))
            {
                if (target.DetectionState > this.minDetectionLevel)
                {
                    if (target.DetectionState > this.chaseDetectionRate)
                        yield return mover.MoveTo(target.Position, mover.RunForce);
                    else
                        yield return mover.MoveTo(target.Position, mover.WalkForce);
                }
            }

            yield break;
        }
    }
}
