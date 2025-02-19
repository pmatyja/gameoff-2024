using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD;
using OCSFX.EZFMODEditor.Metadata.Deserialization;
using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor
{
    internal static class EZFMODEditorStatics
    {
        private const string DEV_NAME = "OCSFX";
        private const string PACKAGE_NAME = "EZFMOD";
        public const string MENU_ITEM_ROOT = DEV_NAME + "/" + PACKAGE_NAME;
        public const string PLUGIN_FOLDER_PATH = "Assets/Plugins/" + MENU_ITEM_ROOT;
        public const string ASSET_OUTPUT_FOLDER_NAME = "GeneratedAssets";
        public const string ASSET_OUTPUT_FULL_PATH = PLUGIN_FOLDER_PATH + "/" + ASSET_OUTPUT_FOLDER_NAME;

        public static void EnsureDirectoryExists(string directoryPath)
        {
            if (!directoryPath.EndsWith("/")) directoryPath += "/";

            var directoryName = Path.GetDirectoryName(directoryPath);

            if (Directory.Exists(directoryName)) return;

            if (directoryName == null) return;

            Directory.CreateDirectory(directoryName);
            AssetDatabase.Refresh();
        }

        internal static void DeleteEmptyDirectories(string rootDirectory)
        {
            var directories = Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories);
            directories = directories.OrderByDescending(d => d.Length).ToArray();

            var deletedAny = false;
            foreach (var directory in directories)
            {
                if (DeleteDirectoryIfEmpty(directory))
                {
                    deletedAny = true;
                }
            }

            if (deletedAny) AssetDatabase.Refresh();
        }

        private static bool DeleteDirectoryIfEmpty(string directory)
        {
            if (Directory.GetFiles(directory).All(file => Path.GetExtension(file) == ".meta") &&
                Directory.GetDirectories(directory).All(DeleteDirectoryIfEmpty))
            {
                var metaFilePath = directory + ".meta";
                if (File.Exists(metaFilePath))
                {
                    File.Delete(metaFilePath);
                }

                Directory.Delete(directory, false);
                Debug.Log($"Deleted empty directory at {directory}");
                return true;
            }

            return false;
        }

        public static HashSet<string> GetExistingAssetPaths<T>(string rootDirectory) where T : Object
        {
            // Ensure the root directory exists
            if (!Directory.Exists(rootDirectory))
            {
                Debug.LogWarning($"Directory does not exist: {rootDirectory}");
                return new HashSet<string>();
            }

            // Find assets and convert GUIDs to asset paths
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { rootDirectory });
            var assetPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToHashSet();

            return assetPaths;
        }

        public static PARAMETER_ID GetParameterIDFromGUID(FMOD.GUID parameterGuid, bool isGlobal)
        {
            var paramID = new PARAMETER_ID();

            // Load the preview banks if they haven't been loaded yet. This is necessary to get the parameter ID
            if (!EditorUtils.PreviewBanksLoaded)
            {
                EditorUtils.LoadPreviewBanks();
            }

            var studioSystem = EditorUtils.System;

            // If the studio system is invalid, return the default parameter ID
            if (!studioSystem.isValid()) return paramID;

            paramID = isGlobal
                ? GetGlobalParameterIDFromGUID(parameterGuid, studioSystem)
                : GetLocalParameterIDFromGUID(parameterGuid, studioSystem);

            return paramID;
        }

        private static PARAMETER_ID GetGlobalParameterIDFromGUID(FMOD.GUID parameterGuid,
            FMOD.Studio.System studioSystem)
        {
            studioSystem.getParameterDescriptionList(out var globalParameterDescriptions);

            foreach (var paramDescription in globalParameterDescriptions)
            {
                if (paramDescription.guid == parameterGuid)
                {
                    return paramDescription.id;
                }
            }

            return EZFMODRuntimeStatics.INVALID_PARAMETER_ID;
        }

        private static PARAMETER_ID GetLocalParameterIDFromGUID(FMOD.GUID parameterGuid,
            FMOD.Studio.System studioSystem)
        {
            studioSystem.getBankList(out var banks);

            foreach (var bank in banks)
            {
                bank.getEventList(out var eventDescriptions);

                foreach (var eventDesc in eventDescriptions)
                {
                    eventDesc.getParameterDescriptionCount(out var count);

                    for (var i = 0; i < count; i++)
                    {
                        eventDesc.getParameterDescriptionByIndex(i, out var paramDesc);

                        if (paramDesc.guid == parameterGuid)
                        {
                            return paramDesc.id;
                        }
                    }
                }
            }

            return EZFMODRuntimeStatics.INVALID_PARAMETER_ID;
        }

        public static T GetOrCreateScriptableObjectAsset<T>(string path, string name, out bool createdNew)
            where T : ScriptableObject
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(name))
            {
                createdNew = false;
                return null;
            }

            createdNew = false;

            var fullPath = $"{path}/{name}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if (asset) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssetIfDirty(asset);

            createdNew = true;

            return asset;
        }

        [MenuItem(MENU_ITEM_ROOT + "/Reconcile FMOD Project Metadata")]
        public static void ReconcileFmodProjectMetadata()
        {
            EZFMODMetadataDeserializer.DeserializeFMODProjectMetadata();
        }

        [MenuItem(MENU_ITEM_ROOT + "/Settings")]
        public static void SelectSettingsAsset()
        {
            Selection.activeObject = EZFMODSettings.Get();
        }

        [MenuItem(MENU_ITEM_ROOT + "/Setup FMOD Listener(s)")]
        public static void SetFmodAudioListeners()
        {
#if UNITY_6000_0_OR_NEWER
            var audioListenerGameObjects =
                Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Select(listener => listener.gameObject).ToArray();

            var cameraGameObjects =
                Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Select(camera => camera.gameObject).ToArray();
#else
            var audioListenerGameObjects =
 Object.FindObjectsOfType<AudioListener>().Select(listener => listener.gameObject).ToArray();
            var cameraGameObjects = Object.FindObjectsOfType<Camera>().Select(camera => camera.gameObject).ToArray();
#endif //UNITY_6000_0_OR_NEWER

            var gameObjectsToCheck = new List<GameObject>();
            gameObjectsToCheck.AddRange(audioListenerGameObjects);
            gameObjectsToCheck.AddRange(cameraGameObjects);

            foreach (var obj in gameObjectsToCheck)
            {
                var dirty = false;
                if (!obj.TryGetComponent<StudioListener>(out _))
                {
                    Undo.AddComponent<StudioListener>(obj);
                    dirty = true;
                }

                if (obj.TryGetComponent<AudioSource>(out var audioSource))
                {
                    Undo.DestroyObjectImmediate(audioSource);
                    dirty = true;
                }

                if (!dirty) continue;

                Undo.RegisterCompleteObjectUndo(obj, nameof(SetFmodAudioListeners));
                EditorUtility.SetDirty(obj);
            }

            DestroyDuplicateListeners();
        }

        private static void DestroyDuplicateListeners()
        {
#if UNITY_6000_0_OR_NEWER
            var fmodListeners =
                Object.FindObjectsByType<StudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var fmodListeners = Object.FindObjectsOfType<StudioListener>();
#endif //UNITY_6000_0_OR_NEWER

            if (fmodListeners.Length == 0) return;

            var owners = fmodListeners.Select(listener => listener.gameObject).ToHashSet();

            foreach (var obj in owners)
            {
                var listeners = obj.GetComponents<StudioListener>();

                if (listeners.Length <= 1) continue;

                Undo.RegisterCompleteObjectUndo(obj, nameof(DestroyDuplicateListeners));

                for (var i = 1; i < listeners.Length; i++)
                {
                    Undo.DestroyObjectImmediate(listeners[i]);
                }

                EditorUtility.SetDirty(obj);
            }
        }
    }
}