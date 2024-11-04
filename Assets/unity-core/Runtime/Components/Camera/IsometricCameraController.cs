using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class IsometricCameraController : BaseCamera
{
    [SerializeField]
    public Transform CameraHandle;
    public override Quaternion InputOffset => Quaternion.AngleAxis(this.rotation, Vector3.up);

    [Header("Target")]

    [SerializeField]
    public GameObject MainTarget;

    [SerializeField]
    public GameObject TemporaryTarget;

    [Header("Anchor")]

    [SerializeField]
    private bool AnchorEnabled;

    [SerializeField]
    private Vector3 Anchor = Vector3.zero;

    [SerializeField]
    [Range(0.0f, float.MaxValue)]
    private float distanceFromAnchor = 50.0f;
    public float DistanceFromAnchor { get => this.distanceFromAnchor; set => this.distanceFromAnchor = Math.Max(0.0f, value); }

    [Header("Properties")]

    [SerializeField]
    public Vector3 Position;

    [SerializeField]
    public Vector3 Offset;

    [SerializeField]
    [Range(0.0f, 360.0f)]
    private float rotation = 45.0f;
    public float Rotation { get => this.rotation; set => this.rotation = ((int)value / (int)this.rotationStep) * this.rotationStep; }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoom = 1.0f;
    public float Zoom { get => this.zoom; set => this.zoom = Mathf.Clamp01(value); }

    [Header("Position")]

    [SerializeField]
    [Range(1.0f, 360.0f)]
    private float positionSpeed = 15.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float positionSmoothness = 0.125f;

    [Header("Rotation")]

    [SerializeField]
    [Range(0.0f, 360.0f)]
    private float rotationStep = 90.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float rotationSmoothness = 0.125f;

    [Header("Zoom")]

    [SerializeField]
    [Range(1.0f, 30.0f)]
    private float zoomSpeed = 10.0f;

    [SerializeField]
    [Range(1.5f, 10.0f)]
    private float zoomMinDistance = 2.0f;

    [SerializeField]
    [Range(1.5f, 20.0f)]
    private float zoomMaxDistance = 10.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoomSmoothness = 0.015625f;

    [Header("Input")]

    [SerializeField]
    public bool enableInput = true;

    [SerializeField]
    [InputActionMap]
    private string inputMove;

    [SerializeField]
    [InputActionMap]
    private string inputAction;

    [SerializeField]
    [InputActionMap]
    private string inputRotateLeft;

    [SerializeField]
    [InputActionMap]
    private string inputRotateRight;

    [SerializeField]
    [InputActionMap]
    private string inputZoom;

    [Header("Shakes")]

    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float maxWorldSpaceShakeDistance = 20.0f;

    [SerializeField]
    private List<CameraShake> shakes = new();

    private Vector3 targetFeedback;
    private Vector3 zoomFeedback;

    private Vector3 direction;
    private Camera cameraObject;

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

    public void Move(Vector3 direction)
    {
        this.direction = new Vector3(direction.x, 0.0f, direction.z);
    }

    public void RotateLeft()
    {
        this.rotation += this.rotationStep;

        if (this.rotation > 360.0f)
        {
            this.rotation -= 360.0f;
        }
    }

    public void RotateRight()
    {
        this.rotation -= this.rotationStep;

        if (this.rotation < 0.0f)
        {
            this.rotation = 360.0f + this.rotation;
        }
    }

    public void Focus(GameObject target, float rotation, float zoom)
    {
        this.TemporaryTarget = target;
        this.rotation = ((int)rotation / (int)this.rotationStep) * this.rotationStep;
        this.zoom = zoom;
    }

    public void OnZoom(float direction)
    {
        this.zoom = Mathf.Clamp(this.zoom - direction * this.zoomSpeed * Time.deltaTime, 0.0f, 1.0f);
    }

    private void Awake()
    {
        this.cameraObject = this.GetComponentInChildren<Camera>();
    }

    private void OnEnable()
    {
        EventBus.AddListener<ClearCameraTargetEventParameters>(this.OnClearTargetEvent);
        EventBus.AddListener<SetCameraTargetEventParameters>(this.OnSetTargetEvent);
        EventBus.AddListener<ZoomCameraTargetEventParameters>(this.OnZoomEvent);
        EventBus.AddListener<ShakeCameraTargetEventParameters>(this.OnShakeEvent);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<ClearCameraTargetEventParameters>(this.OnClearTargetEvent);
        EventBus.RemoveListener<SetCameraTargetEventParameters>(this.OnSetTargetEvent);
        EventBus.RemoveListener<ZoomCameraTargetEventParameters>(this.OnZoomEvent);
        EventBus.RemoveListener<ShakeCameraTargetEventParameters>(this.OnShakeEvent);
    }

    private void Update()
    {
        this.OnInput();
        this.OnUpdate();
        this.OnUpdateEffects();
    }

    private void OnValidate()
    {
        this.cameraObject = this.GetComponentInChildren<Camera>();

        this.OnUpdate();
    }

    private void OnInput()
    {
        if (this.enableInput == false)
        {
            return;
        }

        var direction = InputManager.Linear(this.inputMove);

        this.Move(new Vector3(direction.x, 0, direction.y));

        if (InputManager.Released(this.inputRotateLeft))
        {
            this.RotateLeft();
        }

        if (InputManager.Released(this.inputRotateRight))
        {
            this.RotateRight();
        }

        //var scroll = Mathf.Abs(InputManager.Linear(this.inputZoom).y);
        var scroll = InputManager.Linear(this.inputZoom).y;

        this.OnZoom(scroll);
    }

    [ContextMenu("Update")]
    private void OnUpdate()
    {
        var target = Quaternion.AngleAxis(this.rotation, Vector3.up) * this.direction * (this.positionSpeed * Time.deltaTime);

        if (this.TemporaryTarget)
        {
            target += this.TemporaryTarget.transform.position;
        }
        else if (this.MainTarget)
        {
            target += this.MainTarget.transform.position;
        }
        else
        {
            target += this.Position;
        }

        this.UpdateCamera(target + this.Offset, this.rotation, this.zoom);
    }

    private void OnUpdateEffects()
    {
        if (this.CameraHandle == false)
        {
            return ;
        }
            
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

        this.CameraHandle.transform.localPosition = displacment;
    }

    private void UpdateCamera(Vector3 target, float rotation, float zoom)
    {
        if (this.AnchorEnabled)
        {
            var from = new Vector3(this.Anchor.x, 0.0f, this.Anchor.z);
            var to = new Vector3(this.Position.x, 0.0f, this.Position.z);

            var distanceTarget = Vector3.Distance(from, to);
            if (distanceTarget >= this.distanceFromAnchor)
            {
                this.Position += (from - to).normalized * ((distanceTarget - this.distanceFromAnchor) * this.positionSmoothness);
            }
        }

        this.transform.position = Vector3.SmoothDamp
        (
            this.transform.position, 
            target, 
            ref this.targetFeedback, 
            this.positionSmoothness
        );

        this.transform.rotation = Quaternion.Lerp
        (
            this.transform.rotation, 
            Quaternion.Euler(0.0f, rotation, 0.0f), 
            this.rotationSmoothness
        );

        var distance = Mathf.Lerp(this.zoomMinDistance, this.zoomMaxDistance, zoom);

        this.cameraObject.transform.localPosition = Vector3.SmoothDamp
        (
            this.cameraObject.transform.localPosition, 
            new Vector3(0.0f, distance, -distance), 
            ref this.zoomFeedback, 
            this.zoomSmoothness
        );
    }

    private void OnClearTargetEvent(object sender, ClearCameraTargetEventParameters parameters)
    {
        this.TemporaryTarget = null;
    }

    private void OnSetTargetEvent(object sender, SetCameraTargetEventParameters parameters)
    {
        this.TemporaryTarget = parameters.Target;
    }

    private void OnZoomEvent(object sender, ZoomCameraTargetEventParameters parameters)
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
