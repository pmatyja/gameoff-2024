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
        private const string _menuItemDirectory = "OCSFX/Editor/";
        private const string _outputPath = "Assets/OCSFX/FMOD";
        private const char _directoryDelimiter = '~';
        
        private static EventCache _fmodCacheSO;
        private static string _fmodCachePath;
        
        private static List<EditorParamRef> _editorParamRefs;
        private static List<EditorEventRef> _editorEventRefs;
        private static List<EditorBankRef> _editorBankRefs;
        
        private static int _assetsCreated;
        private static int _assetsUpdated;
        private static int _assetsDeleted;

        //[MenuItem(_menuItemDirectory + nameof(CopyFMODCacheData))]
        public static void CopyFMODCacheData() 
        {
            _fmodCachePath = $"Assets/{RuntimeUtils.PluginBasePath}/Cache/Editor/FMODStudioCache.asset";
            _fmodCacheSO = AssetDatabase.LoadAssetAtPath<EventCache>(_fmodCachePath);

            _editorParamRefs = _fmodCacheSO.EditorParameters;
            _editorBankRefs = _fmodCacheSO.EditorBanks;
            _editorEventRefs = _fmodCacheSO.EditorEvents;

            _assetsCreated = _assetsDeleted = _assetsUpdated = 0;


            // BANKS

            var fmodBanks = new List<string>();
            foreach (var editorBankRef in _editorBankRefs)
            {
                var formattedName = editorBankRef.StudioPath.Replace("bank:/", "bank" + _directoryDelimiter).Replace('/', _directoryDelimiter);

                //Debug.Log(formattedName);
                fmodBanks.Add(formattedName);
                GenerateBankAsset(formattedName, editorBankRef);
            }
            DeleteOldBankAssets(fmodBanks);

            
            // PARAMETERS

            var fmodParams = new List<string>();
            foreach (var editorParams in _editorParamRefs)
            {
                var formattedName = editorParams.StudioPath.Replace("parameter:/", "parameter" + _directoryDelimiter).Replace('/', _directoryDelimiter);

                //Debug.Log(formattedName);
                fmodParams.Add(formattedName);
                GenerateParameterAsset(formattedName, editorParams);
            }
            DeleteOldParameterAssets(fmodParams);

            
            // EVENTS

            var fmodEvents = new List<string>();
            foreach (var editorEventRef in _editorEventRefs)
            {
                var formattedName = editorEventRef.Path.Replace("event:/", "event" + _directoryDelimiter).Replace('/', _directoryDelimiter);
                
                //Debug.Log(formattedName);
                fmodEvents.Add(formattedName);
                GenerateEventAsset(formattedName, editorEventRef);
            }
            DeleteOldEventAssets(fmodEvents);
            
            AssetDatabase.Refresh();
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

            string parentFolder = _outputPath;
            
            // Make each folder in the path if it doesn't already exist
            for (int i = 0; i < subdirectories.Count; i++)
            {
                var folderSubdirectories = new List<string>();
                for (int j = 0; j <= i; j++)
                {
                    folderSubdirectories.Add(subdirectories[j]);
                }

                var folderPath = _outputPath + string.Join('/', folderSubdirectories);
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

        private static void GenerateEventAsset(string formattedName, EditorEventRef editorEventRef, string directory = _outputPath)
        {
            Debug.Log($"Formatted name: {formattedName}");
            Debug.Log($"Directory: {directory}");
            var path = $"{directory}/{formattedName}.asset";
            Debug.Log($"Path: {path}");

            var fmodEvent = AssetDatabase.LoadAssetAtPath<FmodEvent>(path);

            if (!fmodEvent) {
                fmodEvent = ScriptableObject.CreateInstance<FmodEvent>();
                AssetDatabase.CreateAsset(fmodEvent, path);
                fmodEvent.name = formattedName;
                _assetsCreated++;
            }
            else _assetsUpdated++;
            
            var existingBankObjects = GetAllAtDirectory<FmodBank>(_outputPath).ToList();
            var ocsfxFmodBanks = new List<FmodBank>();
            foreach (var bank in editorEventRef.Banks)
            {
                //Debug.Log($"{fmodEvent} trying {bank}");
                
                var foundBank = existingBankObjects.Find(bankObject => bankObject.Name == bank.Name);
                if (!foundBank) continue;
                ocsfxFmodBanks.Add(foundBank);
            }
            
            var existingParamObjects = GetAllAtDirectory<FmodParameter>(_outputPath).ToList();
            var ocsfxFmodParams = new List<FmodParameter>();
            foreach (var param in editorEventRef.Parameters)
            {
                var foundParam = existingParamObjects.Find(paramObject => paramObject.Name == param.Name);
                if (!foundParam) continue;
                ocsfxFmodParams.Add(foundParam);
            }
            
            fmodEvent.Init(
                editorEventRef.Path,
                editorEventRef.Guid,
                ocsfxFmodBanks,
                editorEventRef.IsStream,
                editorEventRef.Is3D,
                editorEventRef.IsOneShot,
                ocsfxFmodParams,
                editorEventRef.MinDistance,
                editorEventRef.MaxDistance,
                editorEventRef.Length
            );
            
            AssetDatabase.SaveAssetIfDirty(fmodEvent);
        }
        
        private static void DeleteOldEventAssets(List<string> assetNames, string directory = _outputPath) {
            var oldAssets = GetAllAtDirectory<FmodEvent>(directory);

            foreach (var instance in oldAssets) {
                if (assetNames.Contains(instance.name)) continue;
                Debug.Log($"Trash {instance.name}");
                var assetPath = $"{directory}/{instance.name}.asset";
                Debug.Log($"{assetPath}");
                AssetDatabase.MoveAssetToTrash(assetPath);
                _assetsDeleted++;
            }
        }
        
        private static void GenerateBankAsset(string formattedName, EditorBankRef editorBankRef, string directory = _outputPath)
        {
            var path = $"{directory}/{formattedName}.asset";

            var fmodBank = AssetDatabase.LoadAssetAtPath<FmodBank>(path);

            if (!fmodBank)
            {
                fmodBank = ScriptableObject.CreateInstance<FmodBank>();
                AssetDatabase.CreateAsset(fmodBank, path);
                fmodBank.name = formattedName;
                _assetsCreated++;
            }
            else _assetsUpdated++;

            var ocsfxFileSizes = new List<FmodBank.NameValuePair>();
            foreach (var fileSize in editorBankRef.FileSizes)
            {
                ocsfxFileSizes.Add(new FmodBank.NameValuePair(fileSize.Name, fileSize.Value));
            }
            
            fmodBank.Init(
                editorBankRef.Path,
                editorBankRef.Name,
                editorBankRef.StudioPath,
                editorBankRef.LastModified,
                ocsfxFileSizes,
                editorBankRef.Exists);
            
            AssetDatabase.SaveAssetIfDirty(fmodBank);
        }
        
        private static void DeleteOldBankAssets(List<string> assetNames, string directory = _outputPath) {
            var oldAssets = GetAllAtDirectory<FmodBank>(directory);

            foreach (var instance in oldAssets) {
                if (assetNames.Contains(instance.name)) continue;
                Debug.Log($"Trash {instance.name}");
                var assetPath = $"{directory}/{instance.name}.asset";
                Debug.Log($"{assetPath}");
                AssetDatabase.MoveAssetToTrash(assetPath);
                _assetsDeleted++;
            }
        }
        
        private static void GenerateParameterAsset(string formattedName, EditorParamRef editorParamRef, string directory = _outputPath)
        {
            var path = $"{directory}/{formattedName}.asset";

            var fmodParameter = AssetDatabase.LoadAssetAtPath<FmodParameter>(path);

            if (!fmodParameter)
            {
                fmodParameter = ScriptableObject.CreateInstance<FmodParameter>();
                AssetDatabase.CreateAsset(fmodParameter, path);
                fmodParameter.name = formattedName;
                _assetsCreated++;
            }
            else _assetsUpdated++;
            
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
        
        private static void DeleteOldParameterAssets(List<string> assetNames, string directory = _outputPath) {
            var oldAssets = GetAllAtDirectory<FmodParameter>(directory);

            foreach (var instance in oldAssets) {
                if (assetNames.Contains(instance.name)) continue;
                Debug.Log($"Trash {instance.name}");
                var assetPath = $"{directory}/{instance.name}.asset";
                Debug.Log($"{assetPath}");
                AssetDatabase.MoveAssetToTrash(assetPath);
                _assetsDeleted++;
            }
        }

        
        // Helpers

        private static T[] GetAllAtDirectory<T>(string directory)
        {
            directory = directory.Replace("Assets", "");
            ArrayList arrayList = new ArrayList();
            string[] fileEntries = Directory.GetFiles(Application.dataPath + directory);
 
            foreach (string fileName in fileEntries)
            {
                string temp = fileName.Replace("\\", "/");
                int index = temp.LastIndexOf("/");
                string localPath = "Assets/" + directory;
 
                if (index > 0) localPath += temp.Substring(index);
 
                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
 
                if (t != null) arrayList.Add(t);
            }
 
            T[] result = new T[arrayList.Count];
 
            for (int i = 0; i < arrayList.Count; i++)
                result[i] = (T) arrayList[i];

            return result;
        }
    }
}