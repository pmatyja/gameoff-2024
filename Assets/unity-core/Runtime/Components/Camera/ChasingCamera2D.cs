using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ChasingCamera2D : MonoBehaviour
{
    [Header("Target")]

    [SerializeField]
    public GameObject MainTarget;

    [SerializeField]
    public GameObject TemporaryTarget;

    [Header("Properties")]

    [SerializeField]
    [Range(-3f, 3f)]
    private float yOffset = 1.0f;

    [SerializeField]
    [Range(0f, 1f)]
    private float smoothness = 0.125f;

    [SerializeField]
    [Range(0f, 1f)]
    private float zoom = 0.125f;
    public float Zoom { get => this.zoom; set => this.zoom = Mathf.Clamp01(value); }

    [SerializeField]
    [Range(1.5f, 10.0f)]
    private float zoomMin = 2.0f;

    [SerializeField]
    [Range(1.5f, 20.0f)]
    private float zoomMax = 10.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoomDuration = 1.0f;

    [Header("Shakes")]

    [SerializeField]
    private Transform parentCameraHandle;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float maxWorldSpaceShakeDistance = 20.0f;

    [SerializeField]
    private List<CameraShake> shakes = new();

    private Camera cameraRef;
    private Vector3 velocityFeedback;

    public void ClearShakes()
    {
        lock (this.shakes)
        {
            this.shakes.Clear();
        }
    }

    public void AddShake(CameraShake shake)
    {
        lock (this.shakes)
        {
            this.shakes.Add(shake);
        }
    }

    private void Start()
    {
        this.cameraRef = this.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        EventBus.AddListener<ClearCameraTargetEventParameters>(this.OnClearTargetEvent);
        EventBus.AddListener<SetCameraTargetEventParameters>(this.OnSetCameraTarget);
        EventBus.AddListener<ZoomCameraTargetEventParameters>(this.OnZoomCameraTarget);
        EventBus.AddListener<ShakeCameraTargetEventParameters>(this.OnShakeEvent);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<ClearCameraTargetEventParameters>(this.OnClearTargetEvent);
        EventBus.RemoveListener<SetCameraTargetEventParameters>(this.OnSetCameraTarget);
        EventBus.RemoveListener<ZoomCameraTargetEventParameters>(this.OnZoomCameraTarget);
        EventBus.RemoveListener<ShakeCameraTargetEventParameters>(this.OnShakeEvent);
    }

    private void FixedUpdate()
    {
        var target = this.MainTarget;

        if (this.TemporaryTarget != null)
        {
            target = this.TemporaryTarget;
        }

        if (target != null)
        {
            var destination = new Vector3
            (
                target.transform.position.x,
                target.transform.position.y + this.yOffset,
                this.transform.position.z
            );

            if (Vector3.Distance(this.transform.position, destination) > 0.01f)
            {
                this.transform.position = Vector3.SmoothDamp(this.transform.position, destination, ref this.velocityFeedback, this.smoothness);
            }
        }

        this.cameraRef.orthographicSize = Mathf.MoveTowards(this.cameraRef.orthographicSize, Mathf.Lerp(this.zoomMin, this.zoomMax, this.zoom), Time.deltaTime / this.zoomDuration);

        if (this.parentCameraHandle != null)
        {
            var displacment = Vector3.zero;

            lock (this.shakes)
            {
                for (var i = 0; i < this.shakes.Count;)
                {
                    var effect = this.shakes[i];

                    if (effect.HasFinished)
                    {
                        this.shakes.Remove(effect);
                    }
                    else
                    {
                        effect.Update();
                        displacment += effect.Displacement;
                        i++;
                    }
                }
            }

            this.parentCameraHandle.transform.localPosition = displacment;
        }
    }

    private void OnClearTargetEvent(object sender, ClearCameraTargetEventParameters parameters)
    {
        this.TemporaryTarget = null;
    }

    private void OnSetCameraTarget(object sender, SetCameraTargetEventParameters parameters)
    {
        this.TemporaryTarget = parameters.Target;
    }

    private void OnZoomCameraTarget(object sender, ZoomCameraTargetEventParameters parameters)
    {
        this.Zoom = parameters.Zoom;
    }

    private void OnShakeEvent(object sender, ShakeCameraTargetEventParameters parameters)
    {
        if (parameters.WorldSpace)
        {
            var distance = Vector3.Distance(this.transform.position, parameters.Position);
            if (distance > this.maxWorldSpaceShakeDistance)
            {
                return;
            }

            // Scale magnitude based on distance
            parameters.Magnitude *= distance / this.maxWorldSpaceShakeDistance;
        }

        this.AddShake(new CameraShake
        {
            Magnitude = parameters.Magnitude,
            Duration = parameters.Duration
        });
    }

    [ContextMenu("Shake (mag 0.1f)")]
    private void DebugShake1()
    {
        EventBus.Raise(this, new ShakeCameraTargetEventParameters
        {
            Magnitude = 0.1f,
            Duration = 1.0f
        });
    }

    [ContextMenu("Shake (mag 0.2f)")]
    private void DebugShake2()
    {
        EventBus.Raise(this, new ShakeCameraTargetEventParameters
        {
            Magnitude = 0.2f,
            Duration = 1.0f
        });
    }

    [ContextMenu("Shake (mag 0.5f)")]
    private void DebugShake5()
    {
        EventBus.Raise(this, new ShakeCameraTargetEventParameters
        {
            Magnitude = 0.5f,
            Duration = 1.0f
        });
    }
}