using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(IdleBrainActivitySO), menuName = "Lavgine/AI/Attack")]
public class AttackBrainActivitySO : BrainActivitySO
{
    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float attackRange = 1.0f;

    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float attackTime = 2.0f;

    [SerializeField]
    [Range(0.01f, 5.0f)]
    private float jumpHeight = 1.0f;

    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float postAttackDelay = 2.0f;

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
                if (Vector3.Distance(target.Value.Target.transform.position, brain.transform.position) <= this.attackRange)
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
        if (brain.TryGetComponent<Rigidbody2D>(out var body) == false)
        {
            yield break;
        }

        if (this.FindBestTarget(brain, out (Vector3 Position, float DetectionState) target))
        {
            var start = brain.transform.position;
            var end   = target.Position;

            yield return Tween.Once(this.attackTime, 1.0f, t =>
            {
                var y = Tween.Bounce(t);

                var newPosition = Vector3.Lerp
                (
                    new Vector3(start.x,    start.y + this.jumpHeight * y, start.z), 
                    new Vector3(end.x,      end.y   + this.jumpHeight * y, end.z), 
                    t
                );

                body.MovePosition(newPosition);
            });

            yield return Wait.Seconds(this.postAttackDelay);
        }
    }
}