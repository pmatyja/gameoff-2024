using System;
using System.Collections;
using UnityEngine;

namespace Nodes.Actions.Conversation
{
    [Serializable]
    public class AmbientDialogNode : ActionNode
    {
        public override float Width => 400.0f;

        [Range(0, 255)]
        public byte Priority;

        [TextArea(5, 5)]
        public string Text;

        [SerializeReference]
        public VoiceLineSO VoiceLine;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            EventBus.Raise(this, new DialogEventParameters
            {
                Priority = this.Priority,
                Content = this.Text,
                VoiceLine = this.VoiceLine
            });

            yield return this.Out?.ExcuteAsync(context);
        }
    }
}
