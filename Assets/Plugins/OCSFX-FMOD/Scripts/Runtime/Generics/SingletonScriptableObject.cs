using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace OCSFX.Generics
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T: ScriptableObject
    {
        /*===========================================================*/
    
        protected static T _instance;
        protected static readonly string RESOURCE_PATH = typeof(T).Name + "/" + typeof(T).Name;

        public static T Get()
        {
            // Don't allow this for abstract classes
            if (typeof(T).IsAbstract)
            {
                Debug.LogError("Cannot create instance of abstract class " + typeof(T).Name);
                return null;
            }
            
            if (!_instance)
            {
                _instance = GetOrCreate();
            }

            return _instance;
        }

        private static T GetOrCreate()
        {
            var assetInstance = Resources.Load<T>(RESOURCE_PATH);
            if (assetInstance) return assetInstance;
        
#if UNITY_EDITOR
            var relativeFileDirectory = "Assets/Resources/" + RESOURCE_PATH + ".asset";
        
            if (!AssetDatabase.IsValidFolder("Assets/Resources/" + typeof(T).Name))
            {
                AssetDatabase.CreateFolder("Assets/Resources", typeof(T).Name);
            }
        
            assetInstance = CreateInstance<T>();
            assetInstance.name = typeof(T).Name;
            
            AssetDatabase.CreateAsset(assetInstance, relativeFileDirectory);
            AssetDatabase.SaveAssets();
#endif

            return assetInstance;
        }
    }
}