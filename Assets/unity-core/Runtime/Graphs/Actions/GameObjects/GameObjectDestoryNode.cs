using System;
using System.Collections;
using UnityEngine;

namespace Nodes.Actions.GameObjects
{
    [Serializable]
    public class GameObjectDestoryNode : ActionNode
    {
        [HideLabel]
        public GameObject Target;

        [Range(0.0f, 60.0f)]
        public float Delay;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            GameObject.Destroy(this.Target, this.Delay);
            yield return base.ExcuteAsync(context);
        }
    }
}