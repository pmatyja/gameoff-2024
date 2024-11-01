using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TypeExtensions
{
    public static readonly Type VoidType = typeof(void);

    public static readonly ConcurrentDictionary<Type, List<Type>> DerivedTypes = new();
    public static readonly ConcurrentDictionary<Type, List<Type>> DerivedManagedSerializableTypes = new();

    public static readonly ConcurrentDictionary<Type, IEnumerable<MemberInfo>> TypeMembers = new();
    public static readonly ConcurrentDictionary<Type, IEnumerable<MethodInfo>> TypeMethods = new();
    public static readonly ConcurrentDictionary<Type, IEnumerable<ReferenceInfo>> TypeReferences = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RunOnStart()
    {
        DerivedTypes.Clear();
        DerivedManagedSerializableTypes.Clear();
        TypeMembers.Clear();
        TypeMembers.Clear();
        TypeMethods.Clear();
        TypeReferences.Clear();
    }

    public static bool IsArray(this Type type)
    {
        return type.IsArray || ( typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string) );
    }

    public static Type GetUnderlyingType(this Type type)
    {
        if (type == null)
        {
            return null;
        }

        var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

        while (nullableUnderlyingType != null)
        {
            type = nullableUnderlyingType;
            nullableUnderlyingType = Nullable.GetUnderlyingType(type);
        }

        return type;
    }

    public static string GetFormattedName(this Type type)
    {
        if (type.IsGenericType == false)
        {
            return type.Name;
        }

        return $"{type.Name.Substring(0, type.Name.IndexOf('`'))}<{string.Join(',', type.GetGenericArguments().Select(x => x.GetFormattedName()))}>";
    }

    public static IEnumerable<Type> GetDerivedTypes(this Type baseType)
    {
        if (baseType == null)
        {
            return Enumerable.Empty<Type>();
        }

        return DerivedTypes.GetOrAdd(baseType, (baseType) =>
        {
            var results = new List<Type>();

            results.Add(baseType);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly
                    .GetTypes()
                    .Where(type => baseType.IsAssignableFrom(type))
                    .OrderBy(type => type.Namespace)
                    .ThenBy(type => type.Name);

                results.AddRange(types);
            }

            return results;
        });
    }

    public static IEnumerable<Type> GetManagedSerializableTypes(this Type baseType)
    {
        if (baseType == null)
        {
            return Enumerable.Empty<Type>();
        }

        return DerivedManagedSerializableTypes.GetOrAdd(baseType, (baseType) =>
        {
            var results = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly
                    .GetTypes()
                    .Where(type => type.IsManagedSerializableType(baseType))
                    .OrderBy(type => type.Namespace)
                    .ThenBy(type => type.Name);

                results.AddRange(types);
            }

            if (baseType.IsAbstract || baseType.IsGenericType || baseType.IsInterface)
            {
                results.Remove(baseType);
            }

            return results;
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsManagedSerializableType(this Type type, Type baseType)
    {
        if ((type.IsPublic || type.IsNestedPublic) && type.IsAbstract == false && type.IsGenericType == false)
        {
            if (Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                return baseType.IsAssignableFrom(type) && typeof(UnityEngine.Object).IsAssignableFrom(type) == false;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetTypeSource(this object parent, string source)
    {
        var member = parent?.GetType()?.GetTypeMember(source);
        return member?.GetValue(parent) as Type;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<ReferenceInfo> GetTypeMembers(this Type baseType)
    {
        return TypeReferences.GetOrAdd(baseType, (baseType) =>
        {
            var members = baseType
                .GetAllMembers()
                .Where(x => x is FieldInfo || x is PropertyInfo)
                .Select(x => ReferenceInfo.Create(x));

            return members;
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<ReferenceInfo> GetTypeMembers(this Type baseType, Type memberType)
    {
        if (baseType == null || memberType == null)
        {
            return Enumerable.Empty<ReferenceInfo>();
        }

        return baseType
            .GetTypeMembers()
            .Where(x => memberType.IsAssignableFrom(x.Type));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReferenceInfo GetTypeMember(this Type baseType, string name)
    {
        if (baseType == null)
        {
            return null;
        }

        return baseType
            .GetTypeMembers()
            .FirstOrDefault(x => x.Name == name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodInfo GetTypeMethod(this Type baseType, string name)
    {
        var member = baseType
            .GetTypeMethods()
            .FirstOrDefault(x => x is MethodInfo && x.Name == name);

        if (member != null)
        {
            return member;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<MethodInfo> GetTypeMethods(this Type baseType)
    {
        return GetTypeMethods(baseType, null, null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<MethodInfo> GetTypeMethods(this Type baseType, Type[] parameters)
    {
        return GetTypeMethods(baseType, null, parameters);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<MethodInfo> GetTypeMethods(this Type baseType, Type returnType)
    {
        return GetTypeMethods(baseType, returnType, null);
    }

    public static IEnumerable<MethodInfo> GetTypeMethods(this Type baseType, Type returnType, Type[] parameters)
    {
        if (baseType == null)
        {
            return Enumerable.Empty<MethodInfo>();
        }

        var methods = TypeMethods.GetOrAdd(baseType, (baseType) =>
        {
            return baseType
                .GetAllMembers()
                .Where(x => x is MethodInfo && x.Name.StartsWith("set_") == false && x.Name.StartsWith("get_") == false)
                .Cast<MethodInfo>();
        });

        return methods
            .Where(x =>
            {
                if (returnType != null)
                {
                    if (returnType.IsAssignableFrom(x.ReturnType) == false)
                    {
                        return false;
                    }
                }

                if (parameters != null)
                {
                    var methodParamters = x.GetParameters();

                    if (parameters.Length !=  methodParamters.Length)
                    {
                        return false;
                    }

                    for (var i = 0; i < parameters.Length; ++i)
                    {
                        if (methodParamters[i].ParameterType.IsAssignableFrom(parameters[i]) == false)
                        {
                            return false;
                        }
                    }
                }

                return true;
            });
    }

    public static IEnumerable<ReferenceInfo> GetSerializableFields(this Type type)
    {
        return type.GetTypeMembers().Where(field => field.IsSerializable);
    }

    private static IEnumerable<MemberInfo> GetAllMembers(this Type baseType)
    {
        var set = new Dictionary<int, MemberInfo>();

        while (baseType != null)
        {
            var members = baseType
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(x => x is FieldInfo || x is PropertyInfo || x is MethodInfo);

            foreach (var member in members)
            {
                set[member.MetadataToken] = member;
            }

            baseType = baseType.BaseType;
        }

        return set.Select(x => x.Value);
    }
}
