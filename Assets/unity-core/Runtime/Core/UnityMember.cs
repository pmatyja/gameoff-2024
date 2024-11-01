public static class UnityMember
{
    public static bool TryGet(UnityEngine.Object source, string name, ref ReferenceInfo result)
    {
        if (result == null)
        {
            result = source
                ?.GetType()
                ?.GetTypeMember(name);

            return result != null;
        }

        return false;
    }

    public static TMemberType TryGetValue<TMemberType>(UnityEngine.Object source, string name, ref ReferenceInfo result, TMemberType defaultValue = default(TMemberType))
    {
        if (UnityMember.TryGet(source, name, ref result))
        {
            return (TMemberType)result.GetValue(source);
        }

        return defaultValue;
    }
}