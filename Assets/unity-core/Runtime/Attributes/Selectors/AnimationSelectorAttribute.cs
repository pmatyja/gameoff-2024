using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AnimationSelectorAttribute : SelectorAttribute
{
    public string SourceAnimatorField { get; }

    public AnimationSelectorAttribute(string sourceAnimatorField)
    {
        this.SourceAnimatorField = sourceAnimatorField;
    }

    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        if (context?.GetType()?.GetTypeMember(this.SourceAnimatorField) is ReferenceInfo member)
        {
            if (member.TryGetValue<RuntimeAnimatorController>(parent, out var runtimeAnimatorController))
            {
                return EditorOnly.GetAnimationNames(runtimeAnimatorController);
            }

            if (member.TryGetValue<Animator>(parent, out var animator))
            {
                return EditorOnly.GetAnimationNames(animator);
            }
        }

        return Enumerable.Empty<object>();
    }
}