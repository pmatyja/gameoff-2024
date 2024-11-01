namespace Nodes.Value
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    [DisplayName("GameObject")]
    public class GameObjectValueNode : ValueNode<GameObject>
    {
        [SerializeReference]
        [TypeInstanceSelector(Label = LabelState.Hidden)]
        public IGameObjectValueSource Source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override GameObject GetValue()
        {
            return this.InternalGetValue(this.Source);
        }
    }
}