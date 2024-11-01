using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SerializerPropertyExtensions
{
    public static bool IsArray(this SerializedProperty property)
    {
        return property.propertyPath.EndsWith("]");
    }

    public static bool SearchHierarchy(this SerializedProperty parent, Func<SerializedProperty, SerializedProperty, int, bool> onItem)
    {
        if (parent == null)
        {
            return false;
        }

        if (onItem.Invoke(null, parent, -1))
        {
            return true;
        }

        var index = 0;
        var iterator = parent.GetEnumerator();

        while (iterator.MoveNext())
        {
            if (iterator.Current is SerializedProperty item)
            {
                if (onItem.Invoke(parent, item, index++))
                {
                    return true;
                }

                if (item.SearchHierarchy(onItem))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool TryGetParentInstance(this SerializedProperty property, out object parent)
    {
        parent = property.GetParentInstance();
        return parent != null;
    }

    public static bool TryGetParentInstance<T>(this SerializedProperty property, out T parent)
    {
        if (property.GetParentInstance() is T casted)
        {
            parent = casted;
            return true;
        }

        parent = default;
        return false;
    }

    public static object GetParentInstance(this SerializedProperty property)
    {
        var path = property.propertyPath.Replace(".Array.data[", "[");
        var elements = path.Split('.');

        object obj = property.serializedObject.targetObject;

        foreach (var element in elements.Take(elements.Length - 1))
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));

                obj = GetValue(obj, elementName, index);
            }
            else
            {
                obj = GetValue(obj, element);
            }
        }

        return obj;
    }

    public static FieldInfo GetFieldInfo(this SerializedProperty property, string name)
    {
        return property.GetParentInstance().GetType()?.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static PropertyInfo GetPropertyInfo(this SerializedProperty property, string name)
    {
        return property.GetParentInstance().GetType()?.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static bool ValidateValues(this SerializedProperty property, string fieldName, params object[] values)
    {
        var member = property.GetFieldInfo(fieldName);
        if (member == null)
        {
            return false;
        }

        var parent = property.GetParentInstance();
        var instance = member.GetValue(parent);

        if (member.FieldType.IsArray || member.FieldType.IsAssignableFrom(typeof(IEnumerable)))
        {
            Debug.LogError($"'ValidateValue' does not support enumerables");
            return true;
        }

        if (member.FieldType.IsEnum)
        {
            if (Attribute.IsDefined(member.FieldType, typeof(FlagsAttribute)))
            {
                if ((int)instance == -1)
                {
                    return true;
                }

                var options = instance.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());

                foreach (var value in values)
                {
                    if (options.Any(x => string.Equals(x, value?.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        if (member.FieldType == typeof(string))
        {
            if (string.IsNullOrWhiteSpace(instance?.ToString()))
            {
                return true;
            }
        }

        if (values == null)
        {
            return true;
        }

        foreach (var value in values)
        {
            if (instance.Equals(value))
            {
                return true;
            }
        }

        return false;
    }

    private static object GetValue(object source, string name)
    {
        if (source == null)
        {
            return null;
        }

        var info = source?.GetType()?.GetTypeMember(name);
        if (info == null)
        {
            return default;
        }

        return info.GetValue(source);
    }

    private static object GetValue(object source, string name, int index)
    {
        var enumerable = GetValue(source, name) as IEnumerable;

        var enm = enumerable?.GetEnumerator();
        if (enm == null)
        {
            return null;
        }

        while (index-- >= 0)
        {
            enm.MoveNext();
        }

        return enm.Current;
    }
}
