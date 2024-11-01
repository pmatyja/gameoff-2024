using System;
using System.Collections;
using UnityEngine;

namespace Nodes.Actions.Camera
{
    [Serializable]
    public class ZoomCameraTarget : ActionNode
    {
        [Range(0f, 1f)]
        public float Zoom;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            EventBus.Raise(this, new ZoomCameraTargetEventParameters
            {
                Zoom = this.Zoom
            });

            yield return base.ExcuteAsync(context);
        }
    }
}