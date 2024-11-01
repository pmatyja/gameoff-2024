using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [SerializeField] private bool dontDestroyOnLoad;

    public static T Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RunOnStart()
    {
        Instance = null;
    }

    protected virtual void Awake()
    {
        if (Instance && Instance != this)
        {
            UnityEngine.GameObject.Destroy(this.gameObject);
            return;
        }

        Instance = (T)this;

        if (this.dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this);
        }
    }
}