//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class FieldOfViewSensor : Sensor
//{
//    [SerializeField]
//    private EventChannelSO eventChannel;

//    [SerializeField]
//    private Vector3 offset = new(0.0f, 1.65f, 0.0f);

//    [SerializeField]
//    [Range(0.0f, 360.0f)]
//    private float pitch = 30.0f;

//    [SerializeField]
//    [Range(0.0f, 360.0f)]
//    private float angle = 90.0f;

//    [SerializeField]
//    [Range(0.0f, 10.0f)]
//    private float rayLength = 10.0f;

//    [SerializeField]
//    private LayerMask obstacleLayers;

//    [SerializeField]
//    private LayerMask detectLayers;

//    [SerializeField]
//    [Range(0.01f, 10.0f)]
//    private float detectionRate = 0.25f;

//    [SerializeField]
//    [Range(0.01f, 10.0f)]
//    private float calmRate = 0.25f;

//    [SerializeField]
//    [Readonly]
//    private Dictionary<GameObject, TrackedTarget> targets = new();
//    public override IReadOnlyDictionary<GameObject, TrackedTarget> Targets => this.targets;

//    [Header("Debug")]

//    [SerializeField]
//    private bool debugEnabled;

//    [SerializeField]
//    private Color rayColor = Color.yellow;

//    private void Update()
//    {
//        foreach (var target in this.targets)
//        {
//            target.Value.Calm(this.calmRate);
//        }

//        Collision.FieldOfView(this.transform.position + this.offset, Quaternion.AngleAxis(this.pitch, this.transform.right) * this.transform.forward, this.transform.up, this.angle, this.rayLength, this.detectLayers, this.obstacleLayers, collider =>
//        {
//            if (collider.gameObject.TryGetComponent<DetectionTarget>(out var detectionTarget))
//            {
//                if (this.targets.ContainsKey(collider.gameObject) == false)
//                {
//                    this.targets.TryAdd(collider.gameObject, new TrackedTarget(detectionTarget));
//                }

//                var target = this.targets[collider.gameObject];

//                target.Detect(this.detectionRate);
//            }
//        });

//        foreach (var target in this.targets)
//        {
//            if (target.Value.DetectionState > 0.0f)
//            {
//                EventBus.Raise<SensorEventParameters>(new
//                {
//                    Sensor = this,
//                    Target = target.Key,
//                    DetectionFactor = target.Value.DetectionState
//                });
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        if (this.debugEnabled)
//        {
//            this.OnDrawGizmosSelected();
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.white;

//        Gizmos.DrawWireSphere(this.transform.position + this.offset, this.rayLength);

//        Gizmos.color = this.rayColor;

//        var forward = (Quaternion.AngleAxis(this.pitch, this.transform.right) * this.transform.forward * this.rayLength);

//        DebugExtension.DrawArrow(this.transform.position + this.offset, forward, this.rayColor);

//        Gizmos.DrawRay(this.transform.position + this.offset, Quaternion.AngleAxis( this.angle / 2.0f, this.transform.up)    * forward);
//        Gizmos.DrawRay(this.transform.position + this.offset, Quaternion.AngleAxis(-this.angle / 2.0f, this.transform.up)    * forward);
//        Gizmos.DrawRay(this.transform.position + this.offset, Quaternion.AngleAxis( this.angle / 2.0f, this.transform.right) * forward);
//        Gizmos.DrawRay(this.transform.position + this.offset, Quaternion.AngleAxis(-this.angle / 2.0f, this.transform.right) * forward);

//        Gizmos.color = Color.red;

//        Collision.FieldOfView(this.transform.position + this.offset, Quaternion.AngleAxis(this.pitch, this.transform.right) * this.transform.forward, this.transform.up, this.angle, this.rayLength, this.detectLayers, this.obstacleLayers, collider =>
//        {
//            if (collider.gameObject.TryGetComponent<DetectionTarget>(out var response))
//            {
//                if (this.targets.TryGetValue(collider.gameObject, out var target))
//                {
//                    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
//                }
//            }
//            else
//            {
//                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
//            }
//        });
//    }
//}
