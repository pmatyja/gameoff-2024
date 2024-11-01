using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class FieldOfViewSensor2D : Sensor
{
    [SerializeField]
    private SpriteDirection spriteDirection;

    [SerializeField]
    private EventChannelSO eventChannel;

    [SerializeField]
    private Vector3 offset = new(0.0f, 1.65f, 0.0f);

    [SerializeField]
    [Range(0.0f, 360.0f)]
    private float angle = 90.0f;

    [SerializeField]
    [Range(0.0f, 180.0f)]
    private float viewWidth = 30.0f;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float rayLength = 10.0f;

    [SerializeField]
    [Range(1, 15)]
    private int degreesPerRay = 5;

    [SerializeField]
    private LayerMask obstacle;

    [SerializeField]
    private LayerMask detect;

    [SerializeField]
    [Range(0.01f, 10.0f)]
    private float detectionRate = 0.2f;

    [SerializeField]
    [Range(0.01f, 10.0f)]
    private float calmRate = 0.2f;

    private SerializedDictionary<GameObject, TrackedTarget> targets = new();
    public override IReadOnlyDictionary<GameObject, TrackedTarget> Targets => this.targets;

    [Header("Debug")]

    [SerializeField]
    private List<TrackedTarget> trackedTargets = new();

    [SerializeField]
    private bool debugEnabled;

    private void Awake()
    {
        if (this.spriteDirection == default)
            this.TryGetComponent(out this.spriteDirection);
    }

    private void Update()
    {
        foreach (var target in this.targets)
        {
            target.Value.Calm(this.calmRate);
        }

        var origin = this.transform.position + this.offset;
        var baseAngle = this.angle;

        if (this.spriteDirection != default)
        {
            if (this.spriteDirection.Direction < 0)
            {
                baseAngle = baseAngle - 180;
            }
        }

        FieldOfView2D
        (
            origin,
            this.rayLength,
            baseAngle.ToRadians(),
            this.viewWidth.ToRadians(),
            this.degreesPerRay,
            this.obstacle,
            this.detect,
        (currentAngle, info) =>
        {
            if (info.collider == default)
            {
                return true;
            }

            if (LayerMaskExtensions.ContainsLayer(detect, info.transform.gameObject.layer))
            {
                if (info.collider.gameObject.TryGetComponent<DetectionTarget>(out var detectionTarget))
                {
                    if (this.targets.ContainsKey(info.collider.gameObject) == false)
                    {
                        this.targets.TryAdd(info.collider.gameObject, new TrackedTarget(detectionTarget));
                    }

                    var target = this.targets[info.collider.gameObject];

                    target.Detect(this.detectionRate);
                }
            }

            return true;
        });

        foreach (var target in this.targets)
        {
            if (target.Value.DetectionState > 0.0f)
            {
                EventBus.Raise<SensorEventParameters>(this.gameObject, new SensorEventParameters
                {
                    Sensor = this,
                    Target = target.Key,
                    DetectionFactor = target.Value.DetectionState
                });
            }

            EditorOnly.Invoke(() =>
            {
                if (this.trackedTargets.Contains(target.Value) == false)
                {
                    this.trackedTargets.Add(target.Value);
                }
            });
        }
    }

    private void OnValidate()
    {
        if (this.spriteDirection == default)
            this.TryGetComponent(out this.spriteDirection);
    }

    private void OnDrawGizmos()
    {
        if (this.debugEnabled)
        {
            this.OnDrawGizmosSelected();
        }
    }

    private void OnDrawGizmosSelected()
    {
        var origin = this.transform.position + this.offset;
        var baseAngle = this.angle;

        if (this.spriteDirection != default)
        {
            if (this.spriteDirection.Direction < 0)
            {
                baseAngle = baseAngle - 180;
            }
        }

        DebugExtension.DrawArrow
        (
            origin,
            new Vector3(Mathf.Cos(baseAngle), Mathf.Sin(baseAngle), 0.0f),
            Color.white
        );

        FieldOfView2D
        (
            origin, 
            this.rayLength,
            baseAngle.ToRadians(), 
            this.viewWidth.ToRadians(), 
            this.degreesPerRay, 
            this.obstacle, 
            this.detect,
            
        (currentAngle, info) =>
        {
            var target = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0.0f);

            if (info.collider == default)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(origin, origin + target * this.rayLength);
            }
            else
            {
                if (LayerMaskExtensions.ContainsLayer(detect, info.transform.gameObject.layer))
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawLine(origin, origin + target * info.distance);
            }

            return true;
        });
    }

    private static void FieldOfView2D(Vector3 origin, float range, float angle, float viewWidth, int degreesPerRay, LayerMask obstacle, LayerMask detect, Func<float, RaycastHit2D, bool> onRay)
    {
        var baseVector = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);

        {
            var target = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);
            var info = Physics2D.Raycast(origin, target, range, obstacle.value | detect.value);

            if (onRay(angle, info) == false)
            {
                return;
            }
        }

        var rayCount   = Math.Max(1, (int)(viewWidth.ToDegrees() / degreesPerRay) - 1);
        var stepAngle  = viewWidth / rayCount;

        var leftAngle   = angle - stepAngle;
        var rightAngle  = angle + stepAngle;

        for (var i = 0; i < rayCount; ++i)
        {
            {
                var target = new Vector3(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle), 0.0f);
                var info = Physics2D.Raycast(origin, target, range, obstacle.value | detect.value);

                if (onRay(leftAngle, info) == false)
                {
                    return;
                }

                leftAngle -= stepAngle;
            }

            {
                var target = new Vector3(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle), 0.0f);
                var info = Physics2D.Raycast(origin, target, range, obstacle.value | detect.value);

                if (onRay(rightAngle, info) == false)
                {
                    return;
                }

                rightAngle += stepAngle;
            }
        }
    }
}
