using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TypeMemberSelectorAttribute : SelectorAttribute
{
    public string BaseTypeSource { get; }
    public Type MemberType { get; }

    private string memberTypeSource;

    public TypeMemberSelectorAttribute(string typeSource, Type memberType)
    {
        this.BaseTypeSource = typeSource;
        this.MemberType = memberType;
    }

    public TypeMemberSelectorAttribute(string typeSource, string memberTypeSource)
    {
        this.BaseTypeSource = typeSource;
        this.memberTypeSource = memberTypeSource;
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
            var fieldType = this.MemberType ?? parent.GetTypeSource(this.memberTypeSource);
            var members = type?.GetTypeMembers(fieldType);

            return members;
        }

        return Enumerable.Empty<object>();
    }

    public override string GetItemGroup(object item)
    {
        if (item is ReferenceInfo info)
        {
            return this.CreateGroup(info.DeclaringType);
        }

        return null;
    }

    public override string GetItemName(object item)
    {
        return (item as ReferenceInfo)?.Name;
    }

    public override object GetValue(object item)
    {
        return (item as ReferenceInfo)?.Name;
    }
}
