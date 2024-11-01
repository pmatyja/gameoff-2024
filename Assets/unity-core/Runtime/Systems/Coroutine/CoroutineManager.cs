using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class CoroutineManager : Singleton<CoroutineManager>
{
    [SerializeField]
    [Range(15.0f, 32.0f)]
    private float maxFrameBudget = 15.0f;

    [SerializeField]
    private bool debugEnabled = true;

    private float updateTime;
    private string debugInfo = string.Empty;

    private static readonly List<CoroutineTask> Coroutines = new();

    public static ICoroutineTask Start(IEnumerator action, string name, object context = null, CoroutinePriority priority = CoroutinePriority.Normal)
    {
        var iterator = new CoroutineTask(name, context, priority, task => action);

        lock (Coroutines)
        {
            Coroutines.Add(iterator);
        }

        return iterator;
    }

    public static ICoroutineTask Start(Func<ICoroutineTask, IEnumerator> action, string name, object context = null, CoroutinePriority priority = CoroutinePriority.Normal)
    {
        var iterator = new CoroutineTask(name, context, priority, action);

        lock (Coroutines)
        {
            Coroutines.Add(iterator);
        }

        return iterator;
    }

    private void OnDisable()
    {
        lock (Coroutines)
        {
            foreach (var coroutine in Coroutines)
            {
                coroutine.Cancel();
            }

            Coroutines.Clear();
        }

        this.StopAllCoroutines();
    }

    private void Update()
    {
        lock (Coroutines)
        {
            for (var i = 0; i < Coroutines.Count;)
            {
                var coroutine = Coroutines[i];

                if (coroutine.IsAlive)
                {
                    if (coroutine.IsRunning == false)
                    {
                        coroutine.Start(this);
                    }
                }
                else
                {
                    Coroutines.RemoveAt(i);
                    continue;
                }

                i++;
            }

            var frameBudgetBase = Mathf.Max(0.01f, this.maxFrameBudget - (Time.deltaTime * 1000.0f)) / Coroutines
                .Where(x => x.Priority != CoroutinePriority.RealTime)
                .Sum(coroutine => (float)coroutine.Priority);

            foreach (var coroutine in Coroutines)
            {
                coroutine.Update((float)coroutine.Priority * frameBudgetBase);
            }

            this.updateTime = Mathf.Clamp01(this.updateTime + Time.deltaTime);

            if (this.debugEnabled)
            {
                if (this.updateTime >= 0.5f)
                {
                    this.updateTime = 0.0f;
                    this.debugInfo = this.GetDebugInfo(Coroutines);
                }
            }
        }
    }

    private void OnValidate()
    {
        if (this.debugEnabled == false)
        {
            this.debugInfo = string.Empty;
        }
    }

    private void OnGUI()
    {
        if (this.debugEnabled == false)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(this.debugInfo))
        {
            this.debugInfo = "Collecting data...";
        }

        GUI.color = Color.white;
        GUI.backgroundColor = Color.black;

        var rect = GUILayoutUtility.GetRect(new GUIContent(this.debugInfo), GUIStyle.none);
        var spacing = Mathf.Max(35.0f, rect.height);

        var offset = Screen.height / 2 - (spacing / 2);

        GUI.Box(new Rect(10, offset + 10, 280, spacing), GUIContent.none);
        GUI.Label(new Rect(16, offset + 16, 280, spacing), this.debugInfo);
    }

    private string GetDebugInfo(IReadOnlyCollection<CoroutineTask> coroutines)
    {
        if ( this.debugEnabled == false )
        {
            return string.Empty;
        }

        var maxNumberOfTasks = 16;
        var numberOfLines = 0;
        var builder = new StringBuilder();

        foreach (var coroutine in coroutines.OrderBy(coroutine => (int)coroutine.Priority))
        {
            if (numberOfLines + 1 > maxNumberOfTasks)
            {
                builder.AppendLine($"   ...");
                break;
            }

            var line = $"{coroutine.Priority}\t {coroutine.Name}";

            if (line.Length > 64)
            {
                line = $"{line.Substring(0, 64)}...";
            }

            builder.AppendLine(line);
            numberOfLines++;
        }

        return builder.ToString();
    }

    [Serializable]
    private class CoroutineTask : ICoroutineTask
    {
        public string Name { get; }
        public bool IsAlive { get; set; } = true;
        public bool IsRunning => this.coroutine != null;
        public object Context { get; }
        public object Result { get; set; }
        public CoroutinePriority Priority { get; set; }

        private MonoBehaviour owner;
        private Coroutine coroutine;
        private float frameBudget;
        private long lastIteratorTime;
        private Func<ICoroutineTask, IEnumerator> action;

        public CoroutineTask(string name, object context, CoroutinePriority priority, Func<ICoroutineTask, IEnumerator> action)
        {
            this.Name = name ?? Guid.NewGuid().ToString();
            this.Context = context;
            this.Priority = priority;
            this.action = action;
        }

        public bool CanExecute()
        {
            if (this.IsAlive && this.frameBudget > 0.0f)
            {
                var currentTime = DateTime.UtcNow.Ticks;

                if (this.lastIteratorTime > 0)
                {
                    this.frameBudget -= (float)(currentTime - this.lastIteratorTime) / TimeSpan.TicksPerMillisecond;
                }

                this.lastIteratorTime = currentTime;
                return true;
            }

            return false;
        }

        public void Cancel()
        {
            this.IsAlive = false;

            if (this.coroutine != null)
            {
                this.owner.StopCoroutine(this.coroutine);
                this.owner = null;
                this.coroutine = null;
            }
        }

        public void Start(MonoBehaviour owner)
        {
            this.IsAlive = owner != null;
            this.owner = owner;
            this.coroutine = owner?.StartCoroutine(this.OnUpdate());
        }

        public void Update(float frameBudget)
        {
            this.frameBudget = Math.Clamp(frameBudget, 0.0f, 15.0f);
            this.lastIteratorTime = 0;
        }

        public IEnumerator OnUpdate()
        {
            yield return null;
            yield return this.action(this);
            this.IsAlive = false;
        }
    }
}