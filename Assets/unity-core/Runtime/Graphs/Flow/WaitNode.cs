using System;
using System.Collections;
using UnityEngine;

namespace Nodes.Branch
{
    [Serializable]
    public class WaitNode : FlowNode
    {
        [HideLabel]
        [Range(0.0f, 3600.0f)]
        public float Seconds;

        [Output]
        public Node Out;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            yield return Wait.Seconds(this.Seconds);
            yield return this.Out?.ExcuteAsync(context);
        }
    }
}