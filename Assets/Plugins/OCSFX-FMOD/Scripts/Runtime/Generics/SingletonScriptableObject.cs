using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace OCSFX.Generics
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T: ScriptableObject
    {
        /*===========================================================*/
    
        private static T _instance;
        private static readonly string RESOURCE_PATH = typeof(T).Name + "/" + typeof(T).Name;

        public static T Get()
        {
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