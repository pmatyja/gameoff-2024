using System.Collections;
using UnityEngine;

public interface IPlatformMover
{
    float WalkForce { get; }
    float RunForce { get; }
    float MaxAcceleration { get; }

    IEnumerator MoveTo(Vector3 position, float maxSpeed);
}
