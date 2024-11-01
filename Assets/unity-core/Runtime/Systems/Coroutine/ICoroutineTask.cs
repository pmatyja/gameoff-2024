public interface ICoroutineTask
{
    bool IsAlive { get; }
    object Context { get; }
    object Result { get; set; }
    CoroutinePriority Priority { get; set; }

    bool CanExecute();
    void Cancel();
}