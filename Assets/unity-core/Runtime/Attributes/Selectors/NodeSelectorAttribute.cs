using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NodeSelectorAttribute : SelectorAttribute
{
    public Type BaseType { get; }

    public NodeSelectorAttribute(Type baseType)
    {
        this.BaseType = baseType;
    }

    public override string GetSelectedItem(object item)
    {
        return NodeGraphCache.GetNodeName(item as Type);
    }

    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        return TypeExtensions.GetManagedSerializableTypes(this.BaseType);
    }

    public override string GetItemGroup(object item)
    {
        return NodeGraphCache.GetNodePath(item as Type);
    }

    public override Texture2D GetItemIcon(object item)
    {
        return EditorOnly.GetIcon(item, "DotFill");
    }

    public override string GetItemName(object item)
    {
        return NodeGraphCache.GetNodeName(item as Type);
    }

    public override object GetValue(object item)
    {
        return item;
    }
}
