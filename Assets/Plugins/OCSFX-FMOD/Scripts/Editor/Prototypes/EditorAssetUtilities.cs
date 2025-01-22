using FMODUnity;
using UnityEngine;

namespace OCSFXEditor.FMOD.Prototype
{
    public static class EditorAssetUtilities
    {
        public static EventCache GetFmodEventCache()
        {
            var fmodEventCachePath = $"{RuntimeUtils.PluginBasePath}/Cache/Editor/FMODStudioCache.asset";
            var fmodEventCache = UnityEditor.AssetDatabase.LoadAssetAtPath<EventCache>(fmodEventCachePath);

            if (!fmodEventCache)
            {
                Debug.LogError($"[{nameof(EditorGenerateFMODScriptableObjects)} | {nameof(GetFmodEventCache)}] FMOD Studio Cache not found at {fmodEventCachePath}");
            }
            
            return fmodEventCache;
        }
        
        public static string GetOrAddFolder(string path, string folderName)
        {
            // Check if the folder exists.
            if (UnityEditor.AssetDatabase.IsValidFolder(path))
            {
                return $"{path}/{folderName}";
            }
            
            // If not, create every folder (if needed) in the path.
            var folders = path.Split('/');
            var currentPath = "Assets";
            foreach (var folder in folders)
            {
                currentPath += $"/{folder}";
                if (!UnityEditor.AssetDatabase.IsValidFolder(currentPath))
                {
                    UnityEditor.AssetDatabase.CreateFolder(currentPath, folder);
                }
            }

            return $"{path}/{folderName}";
        }
        
        public static T CreateScriptableObject<T>(string path, string name) where T : ScriptableObject
        {
            var assetPath = $"{path}/{name}.asset";
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (asset)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            return asset;
        }
        
    }
}