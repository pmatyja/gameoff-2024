using System;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.Utility.Generics
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        [SerializeField] protected bool _dontDestroyOnLoad = true;
        [SerializeField] protected SingletonBehavior _singletonBehavior = SingletonBehavior.PreventNew;
        
        [Space]
        [SerializeField] protected bool _showDebug;
        
        // ReSharper disable once MemberCanBePrivate.Global
        protected static T _instance;

        // ReSharper disable once MemberCanBeProtected.Global
        public static T Instance
        {
            get
            {
                if (Application.isPlaying && !_instance)
                {
                    LazyLoadInstance(out var createdNew);
                    
                    if (createdNew)
                    {
                        OCSFXLogger.Log($"[{nameof(SingletonMonoBehaviour<T>)}] No existing instance of {typeof(T).Name} was found in the scene. " +
                                        "Creating a new instance.", _instance, _instance._showDebug);
                    }
                }
                
                return _instance;
            }
        }
        
        public static void SetDontDestroyOnLoad(bool value)
        {
            if (!_instance) return;
            _instance._dontDestroyOnLoad = value;
        }
        
        public static void SetSingletonBehavior(SingletonBehavior value)
        {
            if (!_instance) return;
            _instance._singletonBehavior = value;
        }

        public static event Action OnInitialized;

        protected virtual void Awake() => SetupSingleton();
        protected virtual void OnDestroy() => ClearSelfAsInstance();
        protected virtual void OnApplicationQuit() => ClearSelfAsInstance();
        
        private void ClearSelfAsInstance()
        {
            if (_instance != this) return;
            
            _instance = null;
            OnInitialized = null;
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _instance = (T)this;
            
            OnInitialized?.Invoke();

            if (!_dontDestroyOnLoad) return;

            // Nested objects can't be marked as DontDestroyOnLoad, so we need to unparent it first
            transform.parent = null;
            
            DontDestroyOnLoad(this);
        }

        protected static T LazyLoadInstance(out bool createdNew)
        {
            createdNew = !_instance;
            if (_instance) return _instance;
            
            var existingInstance = 
#if UNITY_6000_0_OR_NEWER
                FindFirstObjectByType<T>();
#else
                FindObjectOfType<T>();
#endif
            createdNew = !existingInstance;
            
            if (existingInstance)
            {
                _instance = existingInstance;
                
                OCSFXLogger.Log(
                    $"[{nameof(SingletonMonoBehaviour<T>)}] An instance of {typeof(T).Name} was found in the scene " +
                    "and assigned to the instance variable.", _instance, _instance._showDebug);
                
                return _instance;
            }

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
