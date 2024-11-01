namespace Nodes.Branch
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class RandomNode : FlowNode
    {
        [Output]
        public List<Node> Out;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            var randNode = this.Out.Skip(UnityEngine.Random.Range(0, this.Out.Count - 1)).FirstOrDefault();

            yield return randNode?.ExcuteAsync(context);
        }
    }
}