using System;
using System.Collections;

namespace Nodes.Actions.Camera
{
    [Serializable]
    public class SetCameraTarget : ActionNode
    {
        [HideLabel]
        public UnityEngine.GameObject Target;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            EventBus.Raise(this, new SetCameraTargetEventParameters
            {
                Target = this.Target
            });

            yield return base.ExcuteAsync(context);
        }
    }
}