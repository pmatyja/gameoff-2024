using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.Serialization;

namespace OCSFX.Generics
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [SerializeField] protected bool _dontDestroyOnLoad = true;
        [FormerlySerializedAs("singletonBehavior")] [FormerlySerializedAs("_singletonBehaviour")] [SerializeField] protected SingletonBehavior _singletonBehavior = SingletonBehavior.PreventNew;
        
        // ReSharper disable once MemberCanBePrivate.Global
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (Application.isPlaying && !_instance)
                {
                    LazyLoadInstance();
                }
                
                return _instance;
            }
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
                switch (_singletonBehavior)
                {
                    case SingletonBehavior.PreventNew:
                        Destroy(gameObject);
                        return;
                    case SingletonBehavior.DestroyOld:
                        Destroy(_instance.gameObject);
                        break;
                }
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
                
                OCSFXLogger.Log($"An instance of {typeof(T).Name} was found in the scene and assigned to the instance variable.", _instance);
                return _instance;
            }
            
            OCSFXLogger.Log($"No existing instance of {typeof(T).Name} was found in the scene. Creating a new instance.", _instance);

            var go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
            return _instance;
        }
    }
    
    public enum SingletonBehavior
    {
        PreventNew,
        DestroyOld
    }
}
