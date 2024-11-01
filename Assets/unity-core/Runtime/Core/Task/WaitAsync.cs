using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public static class WaitAsync
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task UntilTrue(Func<bool> predicate)
    {
        while (predicate.Invoke())
        {
            await Task.Yield();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task UntilFalse(Func<bool> predicate)
    {
        while (predicate.Invoke() == false)
        {
            await Task.Yield();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task Seconds(float seconds)
    {
        await Task.Delay((int)(1000 * seconds));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task RandomSeconds(float multiplier)
    {
        var seconds = UnityEngine.Random.value * multiplier;
        await Task.Delay((int)(1000 * seconds));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task RealSeconds(float seconds)
    {
        var end = Time.realtimeSinceStartup + seconds;

        while (Time.realtimeSinceStartup > end)
        {
            await Task.Yield();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task NextFrame()
    {
        var nextFrame = Time.frameCount + 1;

        while (Time.frameCount < nextFrame)
        {
            await Task.Yield();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task Coroutine(IEnumerator enumerator, string name, object context = null, CoroutinePriority priority = CoroutinePriority.RealTime)
    {
        await Coroutine(CoroutineManager.Start(task => enumerator, name, context, priority));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task Coroutine(Func<ICoroutineTask, IEnumerator> action, string name, object context = null, CoroutinePriority priority = CoroutinePriority.RealTime)
    {
        await Coroutine(CoroutineManager.Start(action, name, context, priority));
    }

    public static async Task Async(this IEnumerator enumerator, string name, object context = null, CoroutinePriority priority = CoroutinePriority.RealTime)
    {
        await WaitAsync.Coroutine(task => enumerator, name, context, priority);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task Coroutine(ICoroutineTask coroutineTask)
    {
        while (coroutineTask.IsAlive)
        {
            await Task.Yield();
        }
    }
}