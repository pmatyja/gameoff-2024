using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using FMODUnity;
using OCSFX.FMOD.Prototype;
using ParameterType = OCSFX.FMOD.Prototype.ParameterType;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace OCSFXEditor.FMOD.Prototype
{
    public static class EditorGenerateFMODScriptableObjects
    {
        private const string _MENU_ITEM_DIRECTORY = "OCSFX/Editor/";
        private const char _FILENAME_DIRECTORY_DELIMITER = '~';
        
        private static EventCache _fmodCacheSO;
        private static string _fmodCachePath;
        
        private static List<EditorParamRef> _editorParamRefs;
        private static List<EditorEventRef> _editorEventRefs;
        private static List<EditorBankRef> _editorBankRefs;
        
        private static int _assetsCreated;
        private static int _assetsUpdated;
        private static int _assetsDeleted;

        [MenuItem(_MENU_ITEM_DIRECTORY + nameof(CopyFMODCacheData))]
        public static void CopyFMODCacheData() 
        {
            _fmodCachePath = $"{RuntimeUtils.PluginBasePath}/Cache/Editor/FMODStudioCache.asset";
            _fmodCacheSO = AssetDatabase.LoadAssetAtPath<EventCache>(_fmodCachePath);
            
            if (!_fmodCacheSO)
            {
                Debug.LogError($"[{nameof(EditorGenerateFMODScriptableObjects)} | {nameof(CopyFMODCacheData)}] FMOD Studio Cache not found at {_fmodCachePath}");
                return;
            }

            _editorParamRefs = _fmodCacheSO.EditorParameters;
            _editorBankRefs = _fmodCacheSO.EditorBanks;
            _editorEventRefs = _fmodCacheSO.EditorEvents;

            _assetsCreated = _assetsDeleted = _assetsUpdated = 0;

            
            // Create the FMOD folder if it doesn't exist
            EnsureOutputDirectory();
            
            // BANKS
            
            var fmodBanks = new List<string>();
            foreach (var editorBankRef in _editorBankRefs)
            {
                var formattedName = GetFormattedName(editorBankRef.StudioPath);
            
                //Debug.Log(formattedName);
                fmodBanks.Add(formattedName);
                GenerateBankAsset(formattedName, editorBankRef);
            }
            DeleteOldBankAssets(fmodBanks);
            //
            //
            // // PARAMETERS
            //
            // var fmodParams = new List<string>();
            // foreach (var editorParams in _editorParamRefs)
            // {
            //     var formattedName = GetFormattedName(editorParams.StudioPath);
            //
            //     //Debug.Log(formattedName);
            //     fmodParams.Add(formattedName);
            //     GenerateParameterAsset(formattedName, editorParams);
            // }
            // DeleteOldParameterAssets(fmodParams);

            
            // EVENTS

            var fmodEvents = new List<string>();
            foreach (var editorEventRef in _editorEventRefs)
            {
                var formattedName = GetFormattedName(editorEventRef.Path);
                
                //Debug.Log(formattedName);
                fmodEvents.Add(formattedName);
                GenerateEventAsset(formattedName, editorEventRef);
            }
            DeleteOldEventAssets(fmodEvents);
            
            AssetDatabase.Refresh();
        }

        private enum FmodDataType
        {
            Bank,
            Event,
            Parameter,
            Snapshot
        }
        
        private static FmodDataType GetFmodDataType(string path)
        {
            if (path.Contains("bank")) return FmodDataType.Bank;
            if (path.Contains("event")) return FmodDataType.Event;
            if (path.Contains("parameter")) return FmodDataType.Parameter;
            if (path.Contains("snapshot")) return FmodDataType.Snapshot;
            return FmodDataType.Event;
        }
        
        private static string GetFormattedName(string path)
        {
            var keyword = GetFmodDataType(path).ToString().ToLower();
            return path.Replace($"{keyword}:/", $"{keyword}" + _FILENAME_DIRECTORY_DELIMITER).Replace('/', _FILENAME_DIRECTORY_DELIMITER);
        }

        private static void EnsureOutputDirectory()
        {
            OCSFXEditorUtilities.GetOrAddFolder(OCSFXEditorUtilities.ASSET_OUTPUT_FOLDER_PATH, OCSFXEditorUtilities.ASSET_OUTPUT_FOLDER_NAME);
        }

        private static string CreateFoldersFromRefAssetStudioPath(string refAssetStudioPath)
        {
            var path = refAssetStudioPath.Replace(":", "");
            Debug.Log($"Path: {path}");
            var subdirectories = path.Split('/').ToList();

            var lastIndex = subdirectories.Count - 1;
            Debug.Log($"Last index: {lastIndex}");

            // Remove the object itself from subdirectories list. Rebuild the path.
            subdirectories.RemoveAt(lastIndex);
            path = string.Join('/', subdirectories);
            Debug.Log($"Path: {path}");
            
            // Check all the subdirectories indices
            Debug.Log($"Subdirectories: {subdirectories}");
            for (int i = 0; i < subdirectories.Count; i++)
            {
                Debug.Log($"{i} : {subdirectories[i]}");      
            }

            string parentFolder = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH;
            
            // Make each folder in the path if it doesn't already exist
            for (int i = 0; i < subdirectories.Count; i++)
            {
                var folderSubdirectories = new List<string>();
                for (int j = 0; j <= i; j++)
                {
                    folderSubdirectories.Add(subdirectories[j]);
                }

                var folderPath = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH + string.Join('/', folderSubdirectories);
                Debug.Log($"Folder path [{i}]: {folderPath}");

                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    Debug.Log($"Create folder: {folderPath}");
                    Debug.Log($"Folder name: {folderSubdirectories[^1]}");
                    AssetDatabase.CreateFolder(parentFolder, folderSubdirectories[^1]);
                }

                parentFolder = folderPath;
            }
            
            // Then move the generated asset into this folder.
            return parentFolder;
        }

        private static void GenerateEventAsset(string formattedName, EditorEventRef editorEventRef, string directory = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH)
        {
            Debug.Log($"Formatted name: {formattedName}");
            Debug.Log($"Directory: {directory}");
            var path = $"{directory}/{formattedName}.asset";
            Debug.Log($"Path: {path}");
            
            var fmodEvent = OCSFXEditorUtilities.GetOrCreateScriptableObjectAsset<FmodEvent>(directory, formattedName, out var createdNew);
            
            if (!createdNew) _assetsUpdated++;
            else _assetsCreated++;
            
            var existingBankObjects = OCSFXEditorUtilities.GetAllAtDirectory<FmodBank>(directory).ToList();
            var ocsfxFmodBanks = new List<FmodBank>();
            foreach (var bank in editorEventRef.Banks)
            {
                //Debug.Log($"{fmodEvent} trying {bank}");
                
                var foundBank = existingBankObjects.Find(bankObject => bankObject.Name == bank.Name);
                if (!foundBank) continue;
                ocsfxFmodBanks.Add(foundBank);
            }
            
            var existingParamObjects = OCSFXEditorUtilities.GetAllAtDirectory<FmodParameter>(directory).ToList();
            var ocsfxFmodParams = new List<FmodParameter>();
            foreach (var param in editorEventRef.Parameters)
            {
                var foundParam = existingParamObjects.Find(paramObject => paramObject.Name == (GetFormattedName(param.Name)));
                if (!foundParam) continue;
                ocsfxFmodParams.Add(foundParam);
            }
            
            fmodEvent.EditorInit(editorEventRef, ocsfxFmodBanks, ocsfxFmodParams);
        }
        
        private static void DeleteOldEventAssets(List<string> assetNames, string directory = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH) 
        {
            OCSFXEditorUtilities.DeleteInvalidScriptableObjectAssets<FmodEvent>(assetNames, directory, out var deletedCount);
            _assetsDeleted += deletedCount;
        }
        
        private static void GenerateBankAsset(string formattedName, EditorBankRef editorBankRef, string directory = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH)
        {
            var fmodBank = OCSFXEditorUtilities.GetOrCreateScriptableObjectAsset<FmodBank>(directory, formattedName, out var createdNew);
            if (!createdNew) _assetsUpdated++;
            else _assetsCreated++;
            
            fmodBank.EditorInit(editorBankRef);
        }
        
        private static void DeleteOldBankAssets(List<string> assetNames, string directory = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH)
        {
            OCSFXEditorUtilities.DeleteInvalidScriptableObjectAssets<FmodBank>(assetNames, directory, out var deletedCount);
            _assetsDeleted += deletedCount;
        }
        
        private static void GenerateParameterAsset(string formattedName, EditorParamRef editorParamRef, string directory = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH)
        {
            var fmodParameter = OCSFXEditorUtilities.GetOrCreateScriptableObjectAsset<FmodParameter>(directory, formattedName, out var createdNew);
            if (!createdNew) _assetsUpdated++;
            else _assetsCreated++;
            
            fmodParameter.Init(
                editorParamRef.Name,
                editorParamRef.StudioPath,
                editorParamRef.Min,
                editorParamRef.Max,
                editorParamRef.Default,
                editorParamRef.ID,
                (ParameterType)editorParamRef.Type,
                editorParamRef.IsGlobal,
                editorParamRef.Labels,
                editorParamRef.Exists
            );
            
            AssetDatabase.SaveAssetIfDirty(fmodParameter);
        }
        
        private static void DeleteOldParameterAssets(List<string> assetNames, string directory = OCSFXEditorUtilities.ASSET_OUTPUT_FULL_PATH)
        {
            OCSFXEditorUtilities.DeleteInvalidScriptableObjectAssets<FmodParameter>(assetNames, directory, out var deletedCount);
            _assetsDeleted += deletedCount;
        }
        
        // Helpers
    }
}