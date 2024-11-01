using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Nodes.Actions.Generic
{
    [Serializable]
    public class InvokeUnityEventNode : ActionNode
    {
        [SerializeField]
        private UnityEvent Action;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.Action?.Invoke();

            yield return base.ExcuteAsync(context);
        }
    }
}
