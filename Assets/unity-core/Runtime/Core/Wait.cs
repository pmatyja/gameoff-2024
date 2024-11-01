using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public static class Wait
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator UntilTrue(Func<bool> predicate)
    {
        while (predicate.Invoke())
        {
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator UntilFalse(Func<bool> predicate)
    {
        while (predicate.Invoke() == false)
        {
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator Seconds(float seconds)
    {
        while (seconds > 0.0f)
        {
            seconds -= Time.deltaTime;
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator RandomSeconds(float multiplier)
    {
        var seconds = UnityEngine.Random.value * multiplier;

        while (seconds > 0.0f)
        {
            seconds -= Time.deltaTime;
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator RealSeconds(float seconds)
    {
        var end = Time.realtimeSinceStartup + seconds;

        while (Time.realtimeSinceStartup > end)
        {
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator ForTask(Task task)
    {
        while (task.IsCompleted == false)
        {
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator ForTask<T>(Task<T> task)
    {
        while (task.IsCompleted == false)
        {
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator ForTasks(params Task[] tasks)
    {
        while (tasks.Any(x => x.IsCompleted == false))
        {
            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator ForTasks<T>(params Task<T>[] tasks)
    {
        while (tasks.Any(x => x.IsCompleted == false))
        {
            yield return null;
        }
    }
}