using System.Runtime.CompilerServices;
using UnityEngine;

public static class QuaternionUtilities
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion ShorterLerp(Quaternion p, Quaternion q, float t)
	{
		if (Quaternion.Dot(p, q) < 0.0f)
		{
			return ShorterLerp(Multiply(p, -1.0f), q, t);
		}
  
		var r = Quaternion.identity;
		
		r.x = p.x * (1f - t) + q.x * (t);
		r.y = p.y * (1f - t) + q.y * (t);
		r.z = p.z * (1f - t) + q.z * (t);
		r.w = p.w * (1f - t) + q.w * (t);
		
		return r;
	}
  
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion ShorterSlerp(Quaternion p, Quaternion q, float t)
	{
		var dot = Quaternion.Dot(p, q);
		if (dot < 0.0f)
		{
			return ShorterLerp(Multiply(p, -1.0f), q, t);
		}

		var angle = Mathf.Acos(dot);
		
		var first = Multiply(p, Mathf.Sin((1f - t) * angle));
		var second = Multiply(q, Mathf.Sin((t) * angle));
		
		var division = 1f / Mathf.Sin(angle);
		
		return Multiply(new Quaternion(first.x + second.x, first.y + second.y, first.z + second.z, first.w + second.w), division);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Quaternion Multiply(Quaternion input, float scalar)
	{
		return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
	}
}