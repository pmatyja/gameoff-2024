namespace Nodes.Value
{
    using System;
    using UnityEngine;

    [Serializable]
    public class IntValueNode : ValueNode<int>
    {
        [SerializeReference]
        [TypeInstanceSelector(Label = LabelState.Hidden)]
        public IIntValueSource Source;

        public override int GetValue()
        {
            return this.InternalGetValue(this.Source);
        }
    }
}