using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(IdleBrainActivitySO), menuName = "Lavgine/AI/Idle")]
public class IdleBrainActivitySO : BrainActivitySO
{
    [SerializeField]
    [Range(0f, 15f)]
    private float minIdleTime = 2.0f;

    [SerializeField]
    [Range(0f, 15f)]
    private float extraRandomIdleTime = 3.0f;

    public override float GetPriority(BrainBehvaiour brain)
    {
        return 0.0f;
    }

    public override bool CanActivate(BrainBehvaiour brain)
    {
        return true;
    }

    public override IEnumerator OnUpdate(BrainBehvaiour brain)
    {
        var time = this.minIdleTime + Random.Range(0.0f, this.extraRandomIdleTime);

        while (brain.IsRunningActivity(this))
        {
            time -= Time.deltaTime;

            if (time <= 0.0f)
            {
                yield break;
            }

            yield return null;
        }
    }
}
