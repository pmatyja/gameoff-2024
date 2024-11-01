using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskBuilder
{
    public static int ThreadCount => Math.Max(2, SystemInfo.processorCount - 1);

    public static async Task Run(Action action)
    {
        await Task.Run(() =>
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        });
    }
    
    public static async Task Run(CancellationToken cancellationToken, Action<CancellationToken> action)
    {
        await Task.Run(() =>
        {
            try
            {
                action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Task canceled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }, cancellationToken);
    }

    public static Task Split(CancellationToken cancellationToken, int totalCount, int workerCount, Action<int, int> action)
    {
        workerCount = Mathf.Max( 1, workerCount );
        
        var blockSize = totalCount / workerCount;
        var tasks = new Task[workerCount];

        for (var i = 0; i < workerCount; ++i)
        {
            var workerId = i;

            tasks[i] = Task.Run(() =>
            {
                try
                {
                    action.Invoke(workerId, blockSize);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            });
        }

        return Task.WhenAll(tasks);
    }
}