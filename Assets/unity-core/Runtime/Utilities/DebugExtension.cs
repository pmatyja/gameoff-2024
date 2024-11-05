using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public static class DebugExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void PushState(Color color, Action action)
	{
        var oldColor = Gizmos.color;
		Gizmos.color = (color == default) ? Color.white : color;
		action.Invoke();
        Gizmos.color = oldColor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UsingTransformtion(Transform transform, Action gizmos)
    {
        UsingTransform(transform.position, transform.rotation, Vector3.zero, gizmos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UsingTransform(Vector3 position, Quaternion rotation, Vector3 scale, Action gizmos)
    {
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(position, rotation, scale));
        gizmos.Invoke();
        GL.PopMatrix();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawPoint(Vector3 position, Color color, float scale = 1.0f)
	{
		PushState(color, () =>
		{
			Gizmos.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale);
			Gizmos.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale);
			Gizmos.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale);
		});
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawPoint(Vector3 position, float scale = 1.0f)
	{
		DrawPoint(position, Color.white, scale);
	}
	
	public static void DrawBounds(Bounds bounds, Color color)
	{
		var center = bounds.center;

		var x = bounds.extents.x;
		var y = bounds.extents.y;
		var z = bounds.extents.z;

		var ruf = center + new Vector3(x, y, z);
		var rub = center + new Vector3(x, y, -z);
		var luf = center + new Vector3(-x, y, z);
		var lub = center + new Vector3(-x, y, -z);

		var rdf = center + new Vector3(x, -y, z);
		var rdb = center + new Vector3(x, -y, -z);
		var lfd = center + new Vector3(-x, -y, z);
		var lbd = center + new Vector3(-x, -y, -z);

        PushState(color, () =>
        {
            Gizmos.DrawLine(ruf, luf);
			Gizmos.DrawLine(ruf, rub);
			Gizmos.DrawLine(luf, lub);
			Gizmos.DrawLine(rub, lub);

			Gizmos.DrawLine(ruf, rdf);
			Gizmos.DrawLine(rub, rdb);
			Gizmos.DrawLine(luf, lfd);
			Gizmos.DrawLine(lub, lbd);

			Gizmos.DrawLine(rdf, lfd);
			Gizmos.DrawLine(rdf, rdb);
			Gizmos.DrawLine(lfd, lbd);
			Gizmos.DrawLine(lbd, rdb);
		});
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawBounds(Bounds bounds)
	{
		DrawBounds(bounds, Color.white);
	}
	
	public static void DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default)
	{
		var lbb = transform.TransformPoint(center+((-size)*0.5f));
		var rbb = transform.TransformPoint(center+(new Vector3(size.x, -size.y, -size.z)*0.5f));
		
		var lbf = transform.TransformPoint(center+(new Vector3(size.x, -size.y, size.z)*0.5f));
		var rbf = transform.TransformPoint(center+(new Vector3(-size.x, -size.y, size.z)*0.5f));
		
		var lub = transform.TransformPoint(center+(new Vector3(-size.x, size.y, -size.z)*0.5f));
		var rub = transform.TransformPoint(center+(new Vector3(size.x, size.y, -size.z)*0.5f));
		
		var luf = transform.TransformPoint(center+((size)*0.5f));
		var ruf = transform.TransformPoint(center+(new Vector3(-size.x, size.y, size.z)*0.5f));

		PushState(color, () =>
		{
			Gizmos.DrawLine(lbb, rbb);
			Gizmos.DrawLine(rbb, lbf);
			Gizmos.DrawLine(lbf, rbf);
			Gizmos.DrawLine(rbf, lbb);

			Gizmos.DrawLine(lub, rub);
			Gizmos.DrawLine(rub, luf);
			Gizmos.DrawLine(luf, ruf);
			Gizmos.DrawLine(ruf, lub);

			Gizmos.DrawLine(lbb, lub);
			Gizmos.DrawLine(rbb, rub);
			Gizmos.DrawLine(lbf, luf);
			Gizmos.DrawLine(rbf, ruf);
		});
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default)
	{
		DrawLocalCube(transform, size, Color.white, center);
	}
	
	public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default)
	{	
		color = (color == default) ? Color.white : color;
		
		var lbb = space.MultiplyPoint3x4(center+((-size)*0.5f));
		var rbb = space.MultiplyPoint3x4(center+(new Vector3(size.x, -size.y, -size.z)*0.5f));
		
		var lbf = space.MultiplyPoint3x4(center+(new Vector3(size.x, -size.y, size.z)*0.5f));
		var rbf = space.MultiplyPoint3x4(center+(new Vector3(-size.x, -size.y, size.z)*0.5f));
		
		var lub = space.MultiplyPoint3x4(center+(new Vector3(-size.x, size.y, -size.z)*0.5f));
		var rub = space.MultiplyPoint3x4(center+(new Vector3(size.x, size.y, -size.z)*0.5f));
		
		var luf = space.MultiplyPoint3x4(center+((size)*0.5f));
		var ruf = space.MultiplyPoint3x4(center+(new Vector3(-size.x, size.y, size.z)*0.5f));

		PushState(color, () =>
		{
			Gizmos.DrawLine(lbb, rbb);
			Gizmos.DrawLine(rbb, lbf);
			Gizmos.DrawLine(lbf, rbf);
			Gizmos.DrawLine(rbf, lbb);

			Gizmos.DrawLine(lub, rub);
			Gizmos.DrawLine(rub, luf);
			Gizmos.DrawLine(luf, ruf);
			Gizmos.DrawLine(ruf, lub);

			Gizmos.DrawLine(lbb, lub);
			Gizmos.DrawLine(rbb, rub);
			Gizmos.DrawLine(lbf, luf);
			Gizmos.DrawLine(rbf, ruf);
		});
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default)
	{
		DrawLocalCube(space, size, Color.white, center);
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawRect(float x, float z, float width, float height, float y = 0.0f)
    {
        Gizmos.DrawWireCube(new Vector3(x + width * 0.5f, y, z + height * 0.5f), new Vector3(width, 0.01f, height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawRect(Rect rectangle, float y = 0.0f)
    {
        DrawRect(rectangle.x, rectangle.y, rectangle.width, rectangle.height, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawRect(RectInt rectangle, float y = 0.0f)
    {
        DrawRect(rectangle.x, rectangle.y, rectangle.width, rectangle.height, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FillRect(float x, float z, float width, float height, float y = 0.0f)
    {
        Gizmos.DrawCube(new Vector3(x + width * 0.5f, y, z + height * 0.5f), new Vector3(width, 0.01f, height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FillRect(Rect rectangle, float y = 0.0f)
    {
        FillRect(rectangle.x, rectangle.y, rectangle.width, rectangle.height, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FillRect(RectInt rectangle, float y = 0.0f)
    {
        FillRect(rectangle.x, rectangle.y, rectangle.width, rectangle.height, y);
    }

    public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f)
	{
		var _up = up.normalized * radius;
		var _forward = Vector3.Slerp(_up, -_up, 0.5f);
		var _right = Vector3.Cross(_up, _forward).normalized*radius;
		
		var matrix = new Matrix4x4();
		
		matrix[0] = _right.x;
		matrix[1] = _right.y;
		matrix[2] = _right.z;
		
		matrix[4] = _up.x;
		matrix[5] = _up.y;
		matrix[6] = _up.z;
		
		matrix[8] = _forward.x;
		matrix[9] = _forward.y;
		matrix[10] = _forward.z;
		
		var _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
		var _nextPoint = Vector3.zero;
		
		color = (color == default) ? Color.white : color;

        PushState(color, () =>
        {
            var sides = Mathf.Clamp(Mathf.RoundToInt(Mathf.Clamp(radius, 2.0f, 6.0f) / 6.0f * 35.0f), 10, 40);

            for (var i = 0; i < sides; i++)
			{
				_nextPoint.x = Mathf.Cos((i*4)*Mathf.Deg2Rad);
				_nextPoint.z = Mathf.Sin((i*4)*Mathf.Deg2Rad);
				_nextPoint.y = 0;
			
				_nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

				Gizmos.DrawLine(_lastPoint, _nextPoint);

				_lastPoint = _nextPoint;
            }
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCircle(Vector3 position, float radius, Color color)
	{
		DrawCircle(position, Vector3.up, color, radius);
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCircle(Vector3 position, Vector3 up, float radius)
	{
		DrawCircle(position, up, Color.white, radius);
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCircle(Vector3 position, float radius)
	{
		DrawCircle(position, Vector3.up, Color.white, radius);
	}

	public static void DrawWireSphere(Vector3 position, float radius, Color color)
	{
		var angle = 10.0f;
		
		var x = new Vector3(position.x, position.y + radius * Mathf.Sin(0), position.z + radius * Mathf.Cos(0));
		var y = new Vector3(position.x + radius * Mathf.Cos(0), position.y, position.z + radius * Mathf.Sin(0));
		var z = new Vector3(position.x + radius * Mathf.Cos(0), position.y + radius * Mathf.Sin(0), position.z);
		
		Vector3 new_x;
		Vector3 new_y;
		Vector3 new_z;

		PushState(color, () =>
		{
			for (var i = 1; i < 37; i++)
			{

				new_x = new Vector3(position.x, position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
				new_y = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
				new_z = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z);

				Gizmos.DrawLine(x, new_x);
				Gizmos.DrawLine(y, new_y);
				Gizmos.DrawLine(z, new_z);

				x = new_x;
				y = new_y;
				z = new_z;
			}
		});
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawWireSphere(Vector3 position, float radius)
	{
		DrawWireSphere(position, radius, Color.white);
	}
	
	public static void DrawCylinder(Vector3 start, Vector3 end, float radius, Color color)
	{
		var up = (end-start).normalized*radius;
		var forward = Vector3.Slerp(up, -up, 0.5f);
		var right = Vector3.Cross(up, forward).normalized*radius;
		
		//Radial circles
		DrawCircle(start, up, color, radius);	
		DrawCircle(end, -up, color, radius);
		DrawCircle((start+end)*0.5f, up, color, radius);

		//Side lines
		PushState(color, () =>
		{
			Gizmos.DrawLine(start + right, end + right);
			Gizmos.DrawLine(start - right, end - right);

			Gizmos.DrawLine(start + forward, end + forward);
			Gizmos.DrawLine(start - forward, end - forward);

			//Start endcap
			Gizmos.DrawLine(start - right, start + right);
			Gizmos.DrawLine(start - forward, start + forward);

			//End endcap
			Gizmos.DrawLine(end - right, end + right);
			Gizmos.DrawLine(end - forward, end + forward);
		});
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCylinder(Vector3 start, Vector3 end, float radius)
	{
		DrawCylinder(start, end, radius, Color.white);
	}
	
	public static void DrawCone(Vector3 position, Vector3 direction, float angle, Color color)
	{
		var length = direction.magnitude;
		
		var _forward = direction;
		var _up = Vector3.Slerp(_forward, -_forward, 0.5f);
		var _right = Vector3.Cross(_forward, _up).normalized*length;
		
		direction = direction.normalized;
		
		var slerpedVector = Vector3.Slerp(_forward, _up, angle/90.0f);
		
		float dist;
		var farPlane = new Plane(-direction, position+_forward);
		var distRay = new Ray(position, slerpedVector);
	
		farPlane.Raycast(distRay, out dist);

		PushState(color, () =>
		{
			Gizmos.DrawRay(position, slerpedVector.normalized * dist);
			Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist);
			Gizmos.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist);
			Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist);
		});
		
		DrawCircle(position+_forward, direction, color, (_forward-(slerpedVector.normalized*dist)).magnitude);
		DrawCircle(position+(_forward*0.5f), direction, color, ((_forward*0.5f)-(slerpedVector.normalized*(dist*0.5f))).magnitude);
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCone(Vector3 position, Vector3 direction, float angle)
	{
		DrawCone(position, direction, angle, Color.white);
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCone(Vector3 position, Color color, float angle)
	{
		DrawCone(position, Vector3.up, angle, color);
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCone(Vector3 position, float angle)
	{
		DrawCone(position, Vector3.up, angle, Color.white);
	}
	
	public static void DrawCapsule(Vector3 start, Vector3 end, float radius, Color color)
	{
		var up = (end-start).normalized*radius;
		var forward = Vector3.Slerp(up, -up, 0.5f);
		var right = Vector3.Cross(up, forward).normalized*radius;
		
		var height = (start-end).magnitude;
		var sideLength = Mathf.Max(0, (height*0.5f)-radius);
		var middle = (end+start)*0.5f;
		
		start = middle+((start-middle).normalized*sideLength);
		end = middle+((end-middle).normalized*sideLength);
		
		//Radial circles
		DrawCircle(start, up, color, radius);	
		DrawCircle(end, -up, color, radius);

		PushState(color, () =>
		{
			//Side lines
			Gizmos.DrawLine(start + right, end + right);
			Gizmos.DrawLine(start - right, end - right);

			Gizmos.DrawLine(start + forward, end + forward);
			Gizmos.DrawLine(start - forward, end - forward);

			for (var i = 1; i < 26; i++)
			{
				//Start endcap
				Gizmos.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start);
				Gizmos.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start);
				Gizmos.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start);
				Gizmos.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start);

				//End endcap
				Gizmos.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end);
				Gizmos.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end);
				Gizmos.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end);
				Gizmos.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end);
			}
		});
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCapsule(Vector3 center, float height, float radius, Color color)
	{
		DrawCapsule(new Vector3(center.x, center.y + height / 2.0f, center.z), new Vector3(center.x, center.y - height / 2.0f, center.z), radius, color);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawCapsule(Vector3 start, Vector3 end, float radius)
	{
		DrawCapsule(start, end, radius, Color.white);
    }

    public static void DrawArrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f)
    {
        DrawArrow(position, direction, Color.white, arrowHeadLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawArrow(Vector3 from, Vector3 direction, Color color, float arrowHeadLength = 0.25f)
    {
        PushState(color, () =>
        {
            Gizmos.DrawRay(from, direction);
            DrawArrowHead(from, direction, arrowHeadLength);
        });
    }

    public static void DrawArrowHead(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        var right	= Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0)  * Vector3.back;
        var left	= Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
        var up		= Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0)  * Vector3.back;
        var down	= Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

        Gizmos.DrawRay(position + direction, right * arrowHeadLength);
        Gizmos.DrawRay(position + direction, left * arrowHeadLength);
        Gizmos.DrawRay(position + direction, up * arrowHeadLength);
        Gizmos.DrawRay(position + direction, down * arrowHeadLength);
    }

    public static void DrawString(string text, Vector3 worldPosition, Color textColor, Vector2 anchor, float textSize = 15f)
    {
        #if UNITY_EDITOR
            var view = UnityEditor.SceneView.currentDrawingSceneView;

            if (!view)
            {
                return;
            }

            var screenPosition = view.camera.WorldToScreenPoint(worldPosition);

            if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || 
                screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth  || 
                screenPosition.z < 0)
            {
                return;
            }

            var pixelRatio = UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x - UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;

            UnityEditor.Handles.BeginGUI();

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = (int)textSize,
                normal = new GUIStyleState() { textColor = textColor }
            };

            var size = style.CalcSize(new GUIContent(text)) * pixelRatio;
            var alignedPosition =
                ((Vector2)screenPosition +
                size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
                Vector2.up * view.camera.pixelHeight;

            GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);

            UnityEditor.Handles.EndGUI();
        #endif
    }
}
