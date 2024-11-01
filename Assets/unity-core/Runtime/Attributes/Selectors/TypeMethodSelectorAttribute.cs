using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public sealed class TypeMethodSelectorAttribute : SelectorAttribute
{
    public string BaseTypeSource { get; }
    public Type ReturnType { get; }
    public Type[] ParameterTypes { get; }

    private string returnTypeSource;

    public TypeMethodSelectorAttribute(string baseTypeSource, Type returnType, params Type[] parametersTypes)
    {
        this.BaseTypeSource = baseTypeSource;
        this.ReturnType = returnType;
        this.ParameterTypes = parametersTypes;
    }

    public TypeMethodSelectorAttribute(string baseTypeSource, string returnTypeSource, params Type[] parametersTypes)
    {
        this.BaseTypeSource = baseTypeSource;
        this.returnTypeSource = returnTypeSource;
        this.ParameterTypes = parametersTypes;
    }

    public override string GetSelectedItem(object item)
    {
        return item?.ToString();
    }

    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        if (context is ReferenceInfo info)
        {
            var member = info.DeclaringType?.GetTypeMember(this.BaseTypeSource);
            var value = member?.GetValue(parent);
            var type = value?.GetType();
            var returnType = this.ReturnType ?? parent.GetTypeSource(this.returnTypeSource);
            var methods = type?.GetTypeMethods(returnType, this.ParameterTypes).ToArray();

            return methods;
        }

        return Enumerable.Empty<object>();
    }

    public override string GetItemGroup(object item)
    {
        if (item is MethodInfo method)
        {
            return this.CreateGroup(method.DeclaringType ?? method.ReflectedType);
        }

        return null;
    }

    public override string GetItemName(object item)
    {
        return (item as MethodInfo)?.Name;
    }

    public override object GetValue(object item)
    {
        return (item as MethodInfo)?.Name;
    }
}
