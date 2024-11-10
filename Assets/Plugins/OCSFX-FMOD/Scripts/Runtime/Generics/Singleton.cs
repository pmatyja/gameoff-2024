using UnityEngine;

namespace OCSFX.Generics
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [SerializeField] protected bool _dontDestroyOnLoad = true;
        
        // ReSharper disable once MemberCanBePrivate.Global
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (Application.isPlaying && !_instance) _instance = LazyLoadInstance();
                return _instance;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            Application.quitting += () => _instance?.ClearSelfAsInstance();
        }

        protected virtual void Awake() => SetupSingleton();
        protected virtual void OnDestroy() => _instance?.ClearSelfAsInstance();
        protected virtual void OnApplicationQuit() => _instance?.ClearSelfAsInstance();
        
        private void ClearSelfAsInstance()
        {
            if (_instance == this) _instance = null;
        }
        
        private void SetupSingleton()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;

            if (!_dontDestroyOnLoad) return;

            // Nested objects can't be marked as DontDestroyOnLoad, so we need to unparent it first
            transform.parent = null;
            
            DontDestroyOnLoad(this);
        }
        
        protected static T LazyLoadInstance()
        {
            if (_instance) return _instance;
            
            var existingInstance = 
#if UNITY_6000_0_OR_NEWER
                FindFirstObjectByType<T>();
#else
                FindObjectOfType<T>();
#endif
            
            if (existingInstance)
            {
                _instance = existingInstance;
                return _instance;
            }

            var go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
            return _instance;
        }
    }
}
