using System.Collections.Generic;
using System.Linq;

public sealed class ChoiceTrackerSelectorAttribute : SelectorAttribute
{
    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        return ChoiceTrackerSO.Instance?.Choices?.Select(x => x.Name) ?? Enumerable.Empty<object>();
    }
}