using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class TypeInstanceSelectorAttribute : SelectorAttribute
{
    public override string GetSelectedItem(object item)
    {
        return this.GetItemName(item?.GetType());
    }

    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        if (context is ReferenceInfo info)
        {
            var type = info.Type.GetUnderlyingType();

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                type = type.GenericTypeArguments[0]?.GetUnderlyingType();
            }

            return TypeExtensions.GetManagedSerializableTypes(type);
        }

        return Enumerable.Empty<object>();
    }

    public override string GetItemGroup(object item)
    {
        return (item as Type)?.Namespace;
    }

    public override string GetItemName(object item)
    {
        if (item is Type type)
        {
            if (type.TryGetAttribute<DisplayNameAttribute>(out var name))
            {
                return name.DisplayName;
            }

            return type.Name;
        }

        return item?.ToString();
    }

    public override object GetValue(object item)
    {
        if (item is Type asType)
        {
            return Activator.CreateInstance(asType);
        }

        return default;
    }
}
