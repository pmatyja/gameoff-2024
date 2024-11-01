using UnityEngine;

public static class CharacterControllerExtensions
{
	public static void MoveExtended(this CharacterController controller, Vector3 desiredMotion, Vector3 externalVelocity, float maxSpeed)
	{
		var appliedVelocity = default(Vector3);

		appliedVelocity.x = Mathf.Clamp(desiredMotion.x, -maxSpeed, maxSpeed);
		appliedVelocity.z = Mathf.Clamp(desiredMotion.z, -maxSpeed, maxSpeed);

		appliedVelocity.x = PhysicsInterpolation.LimitOrAddVelocity(appliedVelocity.x, externalVelocity.x);
		appliedVelocity.y = externalVelocity.y;
		appliedVelocity.z = PhysicsInterpolation.LimitOrAddVelocity(appliedVelocity.z, externalVelocity.z);

		controller.Move(appliedVelocity * Time.deltaTime);
	}
}