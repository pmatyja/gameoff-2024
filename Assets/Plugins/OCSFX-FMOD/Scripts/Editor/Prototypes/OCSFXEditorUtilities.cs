using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OCSFXEditor.FMOD.Prototype
{
    public static class OCSFXEditorUtilities
    {
        public const string ASSET_OUTPUT_FOLDER_PATH = "Assets/OCSFX";
        public const string ASSET_OUTPUT_FOLDER_NAME = "FMOD";
        public const string ASSET_OUTPUT_FULL_PATH = ASSET_OUTPUT_FOLDER_PATH + "/" + ASSET_OUTPUT_FOLDER_NAME;
        
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
            var fullPath = $"{path}/{folderName}";
            
            // Check if the folder exists.
            if (UnityEditor.AssetDatabase.IsValidFolder(fullPath))
            {
                return fullPath;
            }
            
            Debug.Log($"[{typeof(OCSFXEditorUtilities)} | {nameof(GetOrAddFolder)}] Folder [{folderName}] not found at [{path}]");
            Debug.Log($"[{typeof(OCSFXEditorUtilities)} | {nameof(GetOrAddFolder)}] Creating necessary folders for path [{fullPath}]");
            
            // If not, create every folder (if needed) in the path.
            var folders = fullPath.Split('/');
            var startIndex = folders[0] == "Assets" ? 1 : 0;
            var currentPath = "Assets";

            for (int i = startIndex; i < folders.Length; i++)
            {
                var folder = folders[i];
                
                currentPath += $"/{folder}";
                
                Debug.Log($"[{typeof(OCSFXEditorUtilities)} | {nameof(GetOrAddFolder)}] Checking folder {folder} at {currentPath}");

                if (UnityEditor.AssetDatabase.IsValidFolder(currentPath)) continue;
                
                var parentPath = currentPath.Substring(0, currentPath.LastIndexOf('/'));
                UnityEditor.AssetDatabase.CreateFolder(parentPath, folder);
                Debug.Log($"[{typeof(OCSFXEditorUtilities)} | {nameof(GetOrAddFolder)}] Created folder {folder} at {currentPath}");
            }

            return fullPath;
        }
        
        public static T GetOrCreateScriptableObjectAsset<T>(string path, string name) where T : ScriptableObject
        {
            var fullPath = $"{path}/{name}.asset";
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if (asset) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, fullPath);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(asset);

            return asset;
        }
        
        public static T GetOrCreateScriptableObjectAsset<T>(string path, string name, out bool createdNew) where T : ScriptableObject
        {
            createdNew = false;
            
            var fullPath = $"{path}/{name}.asset";
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if (asset) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, fullPath);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(asset);
            
            createdNew = true;

            return asset;
        }
        
        public static void DeleteInvalidScriptableObjectAssets<T>(List<string> validAssetNames, string path, out int deletedCount) where T : ScriptableObject
        {
            var existingAssets = GetAllAtDirectory<T>(path);
            
            deletedCount = 0;
            
            foreach (var existingAsset in existingAssets)
            {
                if (validAssetNames.Contains(existingAsset.name)) continue;
                
                UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(existingAsset));
                var assetPath = $"{path}/{existingAsset.name}.asset";
                
                Debug.Log($"[{typeof(OCSFXEditorUtilities)} | {nameof(DeleteInvalidScriptableObjectAssets)}] Deleted invalid asset at {assetPath}");
                
                deletedCount++;
            }
        }
        
        public static T[] GetAllAtDirectory<T>(string directory) where T: Object
        {
            directory = directory.Replace("Assets", "");
            var arrayList = new ArrayList();
            var fileEntries = Directory.GetFiles(Application.dataPath + directory);
 
            foreach (string fileName in fileEntries)
            {
                string temp = fileName.Replace("\\", "/");
                int index = temp.LastIndexOf("/", StringComparison.Ordinal);
                string localPath = "Assets/" + directory;
 
                if (index > 0) localPath += temp.Substring(index);
 
                var foundObject = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
 
                if (foundObject != null) arrayList.Add(foundObject);
            }
 
            var result = new T[arrayList.Count];
 
            for (var i = 0; i < arrayList.Count; i++)
                result[i] = (T) arrayList[i];

            return result;
        }
        
    }
}