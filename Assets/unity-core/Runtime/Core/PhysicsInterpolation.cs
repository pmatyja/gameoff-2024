using UnityEngine;

public static class PhysicsInterpolation
{
    public static float AccelerateTo(float currentVelocity, float targetVelocity, float maxAccel)
    {
        var deltaV = targetVelocity - currentVelocity;
        var accel = deltaV;

        if (accel > maxAccel * maxAccel)
            accel = accel * maxAccel;

        return accel;
    }

    public static void AccelerateTo(this Rigidbody body, Vector3 targetVelocity, float maxAccel)
    {
        var deltaV = targetVelocity - body.linearVelocity;
        var accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        body.AddForce(accel, ForceMode.Acceleration);
    }

    public static void AccelerateTo(this Rigidbody2D body, Vector2 targetVelocity, float maxAccel)
    {
        var deltaV = targetVelocity - body.linearVelocity;
        var accel = deltaV;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        body.AddForce(accel, ForceMode2D.Impulse);
    }

    public static float GetJumpForceWeight(float height, float weight = 1.0f, float gravity = -9.8f)
    {
        return weight * Mathf.Sqrt(-2.0f * height * gravity);
    }

    public static float GetJumpForce(float height, float jumpTime = 1.0f)
    {
        // Default jump time is 1.0f for 1-2 unit jump height

        var timeToApex = jumpTime / 2.0f;

        return (2.0f * height) / timeToApex;
    }

    public static float GetVerletIntegrationGravity(float height, float jumpTime = 1.0f)
    {
        // Default jump time is 1.0f for 1-2 unit jump height

        var timeToApex = jumpTime / 2.0f;

        return (-2.0f * height) / Mathf.Pow(timeToApex, 2.0f);
    }

    public static float VerletIntegration(float velocity, float acceleration, float min, float max)
    {
        return Mathf.Clamp( ( velocity + velocity + acceleration ) * 0.5f, min, max );
    }

    public static Vector3 VerletIntegration(Vector3 velocity, Vector3 acceleration)
    {
        return ( velocity + velocity + acceleration ) * 0.5f;
    }

    public static Vector3 ApplyVerletIntegration(Vector3 position, Vector3 velocity, Vector3 acceleration, float delta)
    {
        return position + ( velocity + velocity + acceleration ) * 0.5f * delta;
    }

    public static float LimitOrAddVelocity(float current, float desired)
    {
        if (Mathf.Sign(current) == Mathf.Sign(desired))
        {
            if (current < 0.0f)
            {
                return Mathf.Min(current, desired);
            }

            return Mathf.Max(current, desired);
        }

        return current + desired;
    }
}