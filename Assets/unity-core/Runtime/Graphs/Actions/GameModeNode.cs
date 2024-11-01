using System;
using System.Collections;

namespace Nodes.Actions
{
    [Serializable]
    public class GameModeNode : ActionNode
    {
        public override float Width => 192.0f;

        [HideLabel]
        public GameMode Mode;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            GameManager.Mode = this.Mode;
            yield return base.ExcuteAsync(context);
        }
    }
}
