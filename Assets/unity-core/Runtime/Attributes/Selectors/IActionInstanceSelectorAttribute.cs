using System;
using System.Collections.Generic;
using UnityEngine;

public class IActionInstanceSelectorAttribute : TypeInstanceSelectorAttribute
{
    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        return TypeExtensions.GetManagedSerializableTypes(context.Type);
    }

    public override Texture2D GetItemIcon(object item)
    {
        return EditorOnly.GetIcon(item, "cs Script Icon");
    }
}