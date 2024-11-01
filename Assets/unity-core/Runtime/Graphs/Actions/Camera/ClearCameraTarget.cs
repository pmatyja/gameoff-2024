using System;
using System.Collections;

namespace Nodes.Actions.Camera
{
    [Serializable]
    public class ClearCameraTarget : ActionNode
    {
        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            EventBus.Raise(null, new ClearCameraTargetEventParameters());
            yield return base.ExcuteAsync(context);
        }
    }
}