using System.Collections;
using UnityEngine;

public abstract class BrainActivitySO : ScriptableObject
{
    public string AnimatorState;

    [Range(0.0f, 10.0f)]
    public float Cooldown;
    
    public abstract float GetPriority(BrainBehvaiour brain);
    public abstract bool CanActivate(BrainBehvaiour brain);
    public abstract IEnumerator OnUpdate(BrainBehvaiour brain);

    protected bool FindBestTarget(BrainBehvaiour brain, out (Vector3 Position, float DetectionState) result)
    {
        brain.TryGetComponent<FieldOfViewSensor2D>(out var view);
        brain.TryGetComponent<AudioSensor2D>(out var sensor);

        result = default;

        var bestTarget = default(TrackedTarget);

        if (view != default)
        {
            foreach (var target in view.Targets)
            {
                if (bestTarget == default || target.Value.DetectionState > bestTarget.DetectionState)
                {
                    bestTarget = target.Value;
                }
                else if (target.Value.DetectionState == bestTarget.DetectionState)
                {
                    if (Rng.Boolean(ref Rng.Seed))
                    {
                        bestTarget = target.Value;
                    }
                }
            }
        }

        if (sensor != default && sensor.Detected)
        {
            if (bestTarget == default || sensor.DetectionState > bestTarget.DetectionState)
            {
                result = (sensor.DetectedPosition, sensor.DetectionState);
                return true;
            }
            
            if (sensor.DetectionState == bestTarget.DetectionState)
            {
                if (Rng.Boolean(ref Rng.Seed))
                {
                    result = (sensor.DetectedPosition, sensor.DetectionState);
                    return true;
                }
            }
        }

        if (bestTarget != default)
        {
            result = (bestTarget.Target.transform.position, bestTarget.DetectionState);
            return true;
        }

        return false;
    }
}
