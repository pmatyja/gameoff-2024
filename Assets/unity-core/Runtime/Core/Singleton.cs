using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [SerializeField] private bool dontDestroyOnLoad = true;

    public static T Instance;

// BEGIN_DIVERGENCE | ocooper | 250118 | This method fails in WebGL builds. Use OnApplicationQuit instead

    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    // private static void RunOnStart()
    // {
    //     Application.quitting += () => Instance = null;
    // }
    
    private void OnApplicationQuit()
    {
        Instance = null;
    }
// END_DIVERGENCE | ocooper

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