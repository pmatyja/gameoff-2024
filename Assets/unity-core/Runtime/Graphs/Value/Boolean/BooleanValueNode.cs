namespace Nodes.Value
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    public class BooleanValueNode : ValueNode<bool>
    {
        [SerializeReference]
        [TypeInstanceSelector(Label = LabelState.Hidden)]
        public IBooleanValueSource Source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool GetValue()
        {
            return this.InternalGetValue(this.Source);
        }
    }
}