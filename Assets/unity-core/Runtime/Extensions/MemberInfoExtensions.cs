using System;
using System.Linq;
using System.Reflection;

public static class MemberInfoExtensions
{
    public static ReferenceInfo GetReference(this FieldInfo member)
    {
        if (member == null)
        {
            return null;
        }

        return (member.DeclaringType ?? member.ReflectedType).GetTypeMember(member.Name);
    }

    public static ReferenceInfo GetReference(this PropertyInfo member)
    {
        if (member == null)
        {
            return null;
        }

        return (member.DeclaringType ?? member.ReflectedType).GetTypeMember(member.Name);
    }

    public static bool TryGetAttribute<T>(this MemberInfo member, out T attribute) where T : Attribute
    {
        if (member != null)
        {
            var attributes = member.GetCustomAttributes<T>();

            if (attributes != null && attributes.Any())
            {
                attribute = attributes.FirstOrDefault();

                return attribute as T != null;
            }
        }

        attribute = default;
        return false;
    }
}