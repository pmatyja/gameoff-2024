using System;
using System.Collections;
using Nodes.Value;
using UnityEngine;

namespace Nodes.Actions.Camera
{
    [Serializable]
    public class ShakeCameraTarget : ActionNode
    {
        [Parameter(IsOptional = true)]
        public ValueNode<Vector3> WorldSpace;

        [Range(0.0001f, 2.0f)]
        public float Magnitude = 0.15f;

        [Range(0f, 10f)]
        public float Duration = 1.0f;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            EventBus.Raise(this, new ShakeCameraTargetEventParameters
            {
                WorldSpace = this.WorldSpace != null,
                Position = this.WorldSpace?.GetValue() ?? Vector3.zero,
                Magnitude = this.Magnitude,
                Duration = this.Duration
            });

            yield return base.ExcuteAsync(context);
        }
    }
}