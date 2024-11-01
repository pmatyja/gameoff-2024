using System;
using System.Collections.Generic;

public class TypeSelectorAttribute : SelectorAttribute
{
    public Type BaseType { get; }

    public TypeSelectorAttribute(Type baseType)
    {
        this.BaseType = baseType;
    }

    public override string GetSelectedItem(object item)
    {
        return item?.ToString();
    }

    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        return TypeExtensions.GetDerivedTypes(this.BaseType);
    }

    public override string GetItemGroup(object item)
    {
        return (item as Type)?.Namespace;
    }

    public override string GetItemName(object item)
    {
        return (item as Type)?.Name;
    }

    public override object GetValue(object item)
    {
        return (item as Type)?.FullName;
    }
}
