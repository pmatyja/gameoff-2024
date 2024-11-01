using System.Reflection;

public static class UnityMethod
{
    public static readonly object[] NoArguments = new object[0];

    public static bool TryGet(UnityEngine.Object source, string name, ref MethodInfo result)
    {
        if (result == null)
        {
            result = source
                ?.GetType()
                ?.GetTypeMethod(name);

            return result != null;
        }

        return false;
    }

    public static TReturnType TryInvoke<TReturnType>(UnityEngine.Object source, string name, ref MethodInfo result, TReturnType defaultValue = default(TReturnType))
    {
        return UnityMethod.TryInvoke<TReturnType>(source, name, ref result, defaultValue, NoArguments);
    }

    public static TReturnType TryInvoke<TReturnType>(UnityEngine.Object source, string name, ref MethodInfo result, params object[] parameters)
    {
        return UnityMethod.TryInvoke<TReturnType>(source, name, ref result, default(TReturnType), parameters);
    }

    public static TReturnType TryInvoke<TReturnType>(UnityEngine.Object source, string name, ref MethodInfo result, TReturnType defaultValue, params object[] parameters)
    {
        if (UnityMethod.TryGet(source, name, ref result))
        {
            return (TReturnType)result.Invoke(source, parameters);
        }

        return defaultValue;
    }
}