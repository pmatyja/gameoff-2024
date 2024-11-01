using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(IdleBrainActivitySO), menuName = "Lavgine/AI/Alert")]
public class AlertBrainActivitySO : BrainActivitySO
{
    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float range = 1.0f;

    public override bool CanActivate(BrainBehvaiour brain)
    {
        if (brain.TryGetComponent<Rigidbody2D>(out _) == false)
        {
            return false;
        }

        if (brain.TryGetComponent<FieldOfViewSensor2D>(out var view))
        {
            foreach (var target in view.Targets)
            {
                if (Vector3.Distance(target.Value.Target.transform.position, brain.transform.position) <= this.range)
                {
                    return true;
                }
            }
        }

        return default;
    }

    public override float GetPriority(BrainBehvaiour brain)
    {
        return 2.0f;
    }

    public override IEnumerator OnUpdate(BrainBehvaiour brain)
    {
        if (this.FindBestTarget(brain, out (Vector3 Position, float DetectionState) target))
        {
            EventBus.Raise<NoiseEventParameters>(brain, new NoiseEventParameters
            {
                Source = brain.gameObject,
                Position = target.Position
            });
        }

        yield return null;
    }
}