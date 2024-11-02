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
            get => _instance;
            protected set => _instance = value;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _instance = null;
        }

        protected virtual void Awake() => SetupSingleton();
        protected virtual void OnDestroy() => ClearSelfAsInstance();
        protected virtual void OnApplicationQuit() => ClearSelfAsInstance();
        
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
            
            var existingInstance = FindObjectOfType<T>();
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
