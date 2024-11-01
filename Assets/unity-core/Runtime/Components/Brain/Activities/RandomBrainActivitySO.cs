using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(IdleBrainActivitySO), menuName = "Lavgine/AI/Random")]
public class RandomBrainActivitySO : BrainActivitySO
{
    [SerializeField]
    private BrainActivitySO[] activites;

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
        if (this.activites.Length < 1)
        {
            yield break;
        }

        var index    = Random.Range(0, this.activites.Length);
        var activity = this.activites[index];

        yield return activity.OnUpdate(brain);
    }
}
