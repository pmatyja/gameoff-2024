using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(WanderBrainActivitySO), menuName = "Lavgine/AI/Wander")]
public class WanderBrainActivitySO : BrainActivitySO
{
    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float minTravelTime = 1.0f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float maxTravelTime = 5.0f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float minDistance = 1.0f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float maxDistance = 5.0f;

    public override float GetPriority(BrainBehvaiour brain)
    {
        return 0.0f;
    }

    public override bool CanActivate(BrainBehvaiour brain)
    {
        return brain.TryGetComponent<IPlatformMover>(out var _);
    }

    public override IEnumerator OnUpdate(BrainBehvaiour brain)
    {
        if (brain.TryGetComponent<IPlatformMover>(out var mover))
        {
            var time        = Random.Range(this.minTravelTime, this.maxTravelTime);
            var distance    = Random.Range(this.minDistance, this.maxDistance) * Rng.Sign(ref Rng.Seed);
            var target      = brain.transform.position + new Vector3(distance, 0.0f, 0.0f);

            while (brain.IsRunningActivity(this))
            {
                yield return mover.MoveTo(target, mover.WalkForce);

                time -= Time.deltaTime;

                if (time <= 0.0f)
                {
                    yield break;
                }

                yield return null;
            }
        }
    }
}