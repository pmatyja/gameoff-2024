using System;
using System.Collections;
using UnityEngine;

namespace Nodes.Actions.Generic
{
    [Serializable]
    public class InvokeSceneGraphNode : ActionNode
    {
        public override float Width => 320.0f;

        [SerializeField]
        [HideLabel]
        private NodeGraphPlayer Reference;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            yield return this.Reference?.Graph?.ExcuteAsync(context);
            yield return base.ExcuteAsync(context);
        }
    }
}
