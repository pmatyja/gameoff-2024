using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectorAttribute : BaseAttribute
{
    public abstract IEnumerable<object> GetItems(ReferenceInfo context, object parent);

    public virtual string GetSelectedItem(object item)
    {
        return this.GetItemName(item);
    }

    public virtual string GetItemGroup(object item)
    {
        return null;
    }

    public virtual string GetItemName(object item)
    {
        return item?.ToString();
    }

    public virtual Texture2D GetItemIcon(object item)
    {
        return EditorOnly.GetIcon(item);
    }

    public virtual object GetValue(object item)
    {
        return item?.ToString();
    }

    protected string CreateGroup(Type parent, char separator = '/')
    {
        if (parent == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(parent.Namespace))
        {
            return $"{parent.Name}{separator}";
        }

        return $"{parent.Namespace}{separator}{parent.Name}{separator}";
    }
}