using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable enable

public sealed class ReferenceInfo
{
    public readonly static ReferenceInfo Empty = new(typeof(string).GetField(nameof(string.Empty)), typeof(string));

    public readonly string Name;
    public readonly Type DeclaringType;
    public readonly Type Type;
    public readonly bool IsStatic;
    public readonly bool CanRead;
    public readonly bool CanWrite;
    public readonly bool IsSerializable;

    private readonly MemberInfo memberInfo;
    private readonly Func<object?, object?> getter;
    private readonly Action<object?, object?> setter;

    private ReferenceInfo(MemberInfo memberInfo, Type memberType)
    {
        if (memberInfo is PropertyInfo == false && memberInfo is FieldInfo == false)
        {
            throw new ArgumentException($"MemberInfo must be of type '{nameof(PropertyInfo)}' or '{nameof(FieldInfo)}'", nameof(memberInfo));
        }

        this.Name = memberInfo.Name;
        this.DeclaringType = memberInfo.DeclaringType ?? memberInfo.ReflectedType;
        this.Type = Nullable.GetUnderlyingType(memberType) ?? memberType;
        this.IsStatic = false;

        this.memberInfo = memberInfo;
        this.getter = (instance) => default(object?);
        this.setter = (instance, value) => { };

        if (this.memberInfo is PropertyInfo propertyInfo)
        {
            this.IsStatic = (propertyInfo.GetGetMethod()?.IsStatic ?? false) || (propertyInfo.GetSetMethod()?.IsStatic ?? false);
            this.IsSerializable = false;

            if (propertyInfo.CanRead)
            {
                this.CanRead = true;
                this.getter = propertyInfo.GetValue;
            }

            if (propertyInfo.CanWrite)
            {
                this.CanWrite = true;
                this.setter = propertyInfo.SetValue;
            }
        }
        else if (this.memberInfo is FieldInfo fieldInfo)
        {
            this.IsStatic = fieldInfo.IsStatic;
            this.IsSerializable = IsSerializableField(fieldInfo);

            this.CanRead = true;
            this.getter = fieldInfo.GetValue;

            if (fieldInfo.IsInitOnly == false)
            {
                this.CanWrite = true;
                this.setter = fieldInfo.SetValue;
            }
        }
    }

    public static ReferenceInfo? Create(MemberInfo info)
    {
        if (info is PropertyInfo property)
        {
            return new ReferenceInfo(info, property.PropertyType);
        }

        if (info is FieldInfo field)
        {
            return new ReferenceInfo(info, field.FieldType);
        }

        return null;
    }

    public bool HasAttribute<T>()
    {
        return Attribute.IsDefined(this.memberInfo, typeof(T));
    }

    public bool TryGetAttribute<T>(out T attribute) where T : Attribute
    {
        return this.memberInfo.TryGetAttribute(out attribute);
    }

    public bool SetValue(object? instance, object? value)
    {
        if (value.TryConvertTo(this.Type, out object? parsed))
        {
            if (this.IsStatic)
            {
                this.setter.Invoke(null, value);
            }
            else
            {
                if (instance == null)
                {
                    return false;
                }

                if (this.setter == null)
                {
                    return false;
                }

                this.setter.Invoke(instance, value);
            }

            return true;
        }

        return false;
    }

    public object? GetValue(object? instance)
    {
        if (this.IsStatic)
        {
            return this.getter.Invoke(null);
        }

        if (instance == null)
        {
            return default;
        }

        return this.getter?.Invoke(instance);
    }

    public bool TryGetValue<T>(object? instance, out T? result)
    {
        if (this.GetValue(instance) is T value)
        {
            result = value;
            return true;
        }

        result = default;
        return false;
    }

    private static bool IsSerializableField(FieldInfo field)
    {
        if (Attribute.IsDefined(field, typeof(NonSerializedAttribute)))
        {
            return false;
        }

        if (field.IsPrivate)
        {
            if (Attribute.IsDefined(field, typeof(SerializeField)))
            {
                return true;
            }

            if (Attribute.IsDefined(field, typeof(SerializeReference)))
            {
                return true;
            }
        }

        if (field.FieldType.IsAbstract || field.FieldType.IsInterface)
        {
            if (Attribute.IsDefined(field, typeof(SerializeReference)))
            {
                return true;
            }
        }

        return IsSerializableType(field.FieldType);
    }

    private static bool IsSerializableType(Type type)
    {
        if (type.IsGenericTypeDefinition)
        {
            return false;
        }

        if (type.IsPrimitive || type.IsValueType || type.IsEnum || type == typeof(string))
        {
            return true;
        }

        if (Attribute.IsDefined(type, typeof(SerializableAttribute)))
        {
            return true;
        }

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            return true;
        }

        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return IsSerializableType(type.GetGenericArguments()[0]);
            }

            return false;
        }

        return false;
    }
}
