using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

public class ProgressTask
{
    public int ProgressId { get; private set; }
    public bool IsCancelled => Progress.IsCancellable(this.ProgressId);

    private Func<ProgressTask, IEnumerator> iterator;
    private string description;

    private ProgressTask(string description, Func<ProgressTask, IEnumerator> iterator)
    {
        this.ProgressId = Progress.Start(description);
        this.iterator = iterator;
        this.description = description;

        Editor​Coroutine​Utility.StartCoroutine(this.OnUpdate(), this);
    }

    public static ProgressTask Start(string name, Func<ProgressTask, IEnumerator> iterator)
    {
        return new ProgressTask(name, iterator);
    }

    public void Cancel()
    {
        Progress.Cancel(this.ProgressId);
    }

    public void OnProgress(float progress, string description = null)
    {
        Progress.Report(this.ProgressId, progress, description ?? this.description);
    }

    public void OnProgress(int current, int total, string description = null)
    {
        Progress.Report(this.ProgressId, current, total, description ?? this.description);
    }

    private IEnumerator OnUpdate()
    {
        yield return Editor​Coroutine​Utility.StartCoroutine(this.iterator.Invoke(this), this);
        Progress.Remove(this.ProgressId);
    }
}
