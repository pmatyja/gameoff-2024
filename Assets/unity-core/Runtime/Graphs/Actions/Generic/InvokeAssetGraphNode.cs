using System;
using System.Collections;
using UnityEngine;

namespace Nodes.Actions.Generic
{
    [Serializable]
    public class InvokeAssetGraphNode : ActionNode
    {
        public override float Width => 320.0f;

        [SerializeField]
        [HideLabel]
        private NodeGraphAsset File;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            yield return this.File.Graph.ExcuteAsync(context);
            yield return base.ExcuteAsync(context);
        }
    }
}
