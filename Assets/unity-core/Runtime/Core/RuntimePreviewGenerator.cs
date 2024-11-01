using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

public static class RuntimePreviewGenerator
{
	public static Vector3 DefaultCameraPosition = new(0.57735f, -0.57735f, 0.57735f);

	public static Vector3 CameraPosition = new(0.57735f, -0.57735f, 0.57735f);
	public static readonly Vector3 CameraPositionTop = new(0.0f, -0.57735f, 0.0f);

	public static int CacheCapacity = 1024;

	private static readonly ConcurrentDictionary<string, Texture2D> PreviewCache = new();

    private static Camera InternalCamera;
	private static Camera RenderCamera
	{
		get
		{
			if (InternalCamera == null)
			{
				InternalCamera = new GameObject("ModelPreviewGeneratorCamera").AddComponent<Camera>();
				InternalCamera.enabled = false;
				InternalCamera.orthographic = true;
				InternalCamera.nearClipPlane = 0.01f;
				InternalCamera.farClipPlane = 50.0f;
				InternalCamera.cullingMask = 1 << DefaultLayer;
				InternalCamera.clearFlags = CameraClearFlags.Color;
				InternalCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
				InternalCamera.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
			}

			return InternalCamera;
		}
	}

	private const int DefaultLayer = 22;
	private static readonly Vector3[] BoundingBoxPoints = new Vector3[8];
	private static readonly List<Renderer> RenderersList = new( 64 );

	public static void ResetCache()
	{
		PreviewCache.Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Texture2D GenerateModelPreview(PrefabSO prefab, int width, int height, Vector3? camerPosition = null)
	{
		if (prefab)
		{
			return GenerateModelPreview(prefab.Model?.transform, prefab.GetQuaternion(), prefab.Pivot, width, height, camerPosition);
		}

		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Texture2D GenerateModelPreview(PrefabSO prefab, int rotation, int width, int height, Vector3? camerPosition = null)
	{
		if (prefab)
		{
			return GenerateModelPreview(prefab.Model?.transform, prefab.GetQuaternion(rotation), prefab.Pivot, width, height, camerPosition);
		}

		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Texture2D GenerateModelPreview(GameObject model, Quaternion rotation, Vector3 pivot, int width, int height, Vector3? camerPosition = null)
	{
		return GenerateModelPreview(model?.transform, rotation, pivot, width, height, camerPosition);
	}

	public static Texture2D GenerateModelPreview(Transform model, Quaternion rotation, Vector3 pivot, int width, int height, Vector3? camerPosition = null)
	{
		if (model == null)
		{
			return null;
		}

		if (PreviewCache.Count > CacheCapacity)
        {
			PreviewCache.Clear();
		}

		return PreviewCache.GetOrAdd(GetKey(model, rotation, pivot, width, height, camerPosition), key =>
        {
			var activeRT = RenderTexture.active;
			var renderTexture = default(RenderTexture);

			var sun = new GameObject("sun");

			sun.transform.localRotation = Quaternion.Euler(45.0f, 45.0f, 0.0f);
            sun.gameObject.hideFlags = HideFlags.HideAndDontSave;

            var light = sun.gameObject.AddComponent<Light>();

			light.intensity = 0.5f;
            light.type = LightType.Directional;

            var parent = new GameObject("pivot").transform;

			parent.gameObject.hideFlags = HideFlags.HideAndDontSave;

			var previewObject = GameObject.Instantiate(model, parent, false);

            previewObject.gameObject.hideFlags = HideFlags.HideAndDontSave;
            previewObject.transform.localPosition = pivot;
			previewObject.transform.localRotation = Quaternion.identity;
			previewObject.transform.localScale = Vector3.one;

			try
			{
				if (CalculateBounds(previewObject, out var previewBounds) == false)
				{
					return null;
				}

				SetLayerRecursively(parent);

				RenderCamera.aspect = (float)width / height;
				RenderCamera.transform.rotation = Quaternion.LookRotation(camerPosition ?? CameraPosition, Vector3.up);

				CalculateCameraPosition(RenderCamera, previewBounds);

				parent.transform.localRotation = rotation;

				RenderCamera.farClipPlane = (RenderCamera.transform.position - previewBounds.center).magnitude + previewBounds.size.magnitude;

				renderTexture = RenderTexture.GetTemporary(width, height, 16);

				RenderTexture.active = renderTexture;

				RenderCamera.targetTexture = renderTexture;
				RenderCamera.Render();
				RenderCamera.targetTexture = null;

				var result = new Texture2D(width, height, TextureFormat.RGB24, false);
				result.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
				result.Apply(false, false);

				return result;
			}
			catch (Exception e)
			{
                Debug.LogException(e);
			}
			finally
			{
				RenderTexture.active = activeRT;

				if (renderTexture)
				{
					RenderTexture.ReleaseTemporary(renderTexture);
				}

				Object.DestroyImmediate(previewObject.gameObject);
				Object.DestroyImmediate(parent.gameObject);
                Object.DestroyImmediate(sun);
            }

			return null;
		});
	}

    private static string GetKey(Transform model, Quaternion rotation, Vector3 pivot, int width, int height, Vector3? camerPosition = null)
    {
		return $"{model.GetInstanceID()};{model.name};{rotation};{pivot};{width};{height};{camerPosition}";
    }

    public static bool CalculateBounds(Transform target, out Bounds bounds)
	{
		RenderersList.Clear();
		target.GetComponentsInChildren( RenderersList );

		bounds = new Bounds();
		var hasBounds = false;

		foreach (var t in RenderersList)
		{
			if (!t.enabled)
			{
				continue;
			}

			if (t is ParticleSystemRenderer)
			{
				continue;
			}

			if (!hasBounds)
			{
				bounds = t.bounds;
				hasBounds = true;
			}
			else
			{
				bounds.Encapsulate(t.bounds);
			}
		}

		return hasBounds;
	}

	public static void CalculateCameraPosition(Camera camera, Bounds bounds)
	{
		var cameraTR = camera.transform;
		var cameraDirection = cameraTR.forward;
		var aspect = camera.aspect;

		var boundsCenter = bounds.center;
		var boundsExtents = bounds.extents;
		var boundsSize = 2f * boundsExtents;

		// Calculate corner points of the Bounds
		var point = boundsCenter + boundsExtents;
		BoundingBoxPoints[0] = point;
		point.x -= boundsSize.x;
		BoundingBoxPoints[1] = point;
		point.y -= boundsSize.y;
		BoundingBoxPoints[2] = point;
		point.x += boundsSize.x;
		BoundingBoxPoints[3] = point;
		point.z -= boundsSize.z;
		BoundingBoxPoints[4] = point;
		point.x -= boundsSize.x;
		BoundingBoxPoints[5] = point;
		point.y += boundsSize.y;
		BoundingBoxPoints[6] = point;
		point.x += boundsSize.x;
		BoundingBoxPoints[7] = point;

		cameraTR.position = boundsCenter;

		var minX = float.PositiveInfinity;
		var minY = float.PositiveInfinity;
		var maxX = float.NegativeInfinity;
		var maxY = float.NegativeInfinity;

		for (var i = 0; i < BoundingBoxPoints.Length; i++)
		{
			var localPoint = cameraTR.InverseTransformPoint( BoundingBoxPoints[i] );

			if( localPoint.x < minX )
				minX = localPoint.x;
			if( localPoint.x > maxX )
				maxX = localPoint.x;
			if( localPoint.y < minY )
				minY = localPoint.y;
			if( localPoint.y > maxY )
				maxY = localPoint.y;
		}

		var distance = boundsExtents.magnitude + 1f;

		camera.orthographicSize = Mathf.Max( maxY - minY, ( maxX - minX ) / aspect ) * 0.5f;
		cameraTR.position = boundsCenter - cameraDirection * distance;
	}

	private static void SetLayerRecursively(Transform obj)
	{
		obj.gameObject.layer = DefaultLayer;

		for (var i = 0; i < obj.childCount; i++)
		{
			SetLayerRecursively(obj.GetChild(i));
		}
	}
}