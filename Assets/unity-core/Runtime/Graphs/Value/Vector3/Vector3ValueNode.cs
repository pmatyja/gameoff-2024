namespace Nodes.Value
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    [DisplayName("Vector3")]
    public class Vector3ValueNode : ValueNode<Vector3>
    {
        [SerializeReference]
        [TypeInstanceSelector(Label = LabelState.Hidden)]
        public IVector3ValueSource Source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Vector3 GetValue()
        {
            return this.InternalGetValue(this.Source);
        }
    }
}