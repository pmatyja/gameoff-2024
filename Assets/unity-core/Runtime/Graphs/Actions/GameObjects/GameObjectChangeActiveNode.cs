using System;
using System.Collections;

namespace Nodes.Actions.GameObjects
{
    [Serializable]
    public class GameObjectChangeActiveNode : ActionNode
    {
        [HideLabel]
        public UnityEngine.GameObject Target;

        public bool State;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.Target.SetActive(this.State);
            yield return base.ExcuteAsync(context);
        }
    }
}