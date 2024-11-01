using System;
using System.Collections;

namespace Nodes.Actions.Generic
{
    [Serializable]
    public class PrintNode : ActionNode
    {
        [HideLabel]
        public string Text;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            UnityEngine.Debug.Log($"[{DateTime.UtcNow.Millisecond:0000}ms] {this.Text}");
            yield return base.ExcuteAsync(context);
        }
    }
}