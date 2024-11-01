using System.Collections.Generic;

public sealed class SceneSelectorAttribute : SelectorAttribute
{
    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        return EditorOnly.GetAllScenes();
    }
}
