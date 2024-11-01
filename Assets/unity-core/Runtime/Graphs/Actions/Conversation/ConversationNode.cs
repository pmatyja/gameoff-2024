using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nodes.Actions.Conversation
{
    [Serializable]
    public class ConversationNode : ActionNode
    {
        public override float Width => 400.0f;

        [Output]
        private List<ChoiceNode> Choices = new();

        public List<SpriteDefinition> Sprites = new();
        
        [Space]
        [Expandable]
        public ActorSO Actor;

        public GameObject Anchor;

        [TextArea(5, 5)]
        public string Text;

        public ConversationProgression Progression = ConversationProgression.Automatic;

        [SerializeReference]
        public VoiceLineSO VoiceLine;

        public IEnumerable<Choice> GetChoices()
        {
            return this.Choices.OrderBy(x => x.OrderId);
        }

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            var result = new CoroutineResult<ChoiceRef>();

            yield return UserInterface.Instance.PlayConversation(this, this.GetChoices(), result);

            if (this.Choices.Count > 0)
            {
                if (result.Value == null)
                {
                    yield return this.Choices?.FirstOrDefault()?.ExcuteAsync(context);
                }
                else
                {
                    yield return this.Choices[result.Value.Index]?.ExcuteAsync(context);
                }
            }
            else
            {
                yield return this.Out?.ExcuteAsync(context);
            }
        }
    }
}
