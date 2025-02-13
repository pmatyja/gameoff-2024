using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMODEditor.Metadata.Types;
using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor
{
    public class EZFMODScriptableObjectAssetGenerator
    {
        public static bool IsGeneratingAssets { get; private set; }

        private readonly string _assetOutputPath;
        private readonly List<FMODBankData> _banksData;
        private readonly List<FMODParameterData> _parametersData;
        private readonly List<FMODEventData> _eventsData;
        private readonly List<FMODSnapshotData> _snapshotsData;
        
        private EZFMODBank _masterBank;

        private int _generatedAssets;
        private int _modifiedAssets;
        private int _deletedAssets;
        
        private List<EZFMODBank> _bankScriptableObjects;
        private List<EZFMODParameter> _parameterScriptableObjects;
        private List<EZFMODEvent> _eventScriptableObjects;
        private List<EZFMODSnapshot> _snapshotScriptableObjects;

        public EZFMODScriptableObjectAssetGenerator(string assetOutputPath,
            List<FMODBankData> banks,
            List<FMODParameterData> parameters,
            List<FMODEventData> events,
            List<FMODSnapshotData> snapshots)
        {
            _assetOutputPath = assetOutputPath;

            _banksData = banks;
            _parametersData = parameters;
            _eventsData = events;
            _snapshotsData = snapshots;
        }

        public void GenerateScriptableObjects()
        {
            // if (IsGeneratingAssets)
            // {
            //     Debug.LogWarning(
            //         "FMOD scriptable objects are already being generated. Please wait for the process to complete.");
            //     return;
            // }
            //
            // IsGeneratingAssets = true;

            Debug.Log($"[{nameof(EZFMOD)}] Reconciling scriptable objects...");

            _generatedAssets = 0;
            _modifiedAssets = 0;
            _deletedAssets = 0;

            var startTime = EditorApplication.timeSinceStartup;

            EZFMODEditorStatics.EnsureDirectoryExists(_assetOutputPath);

            GenerateBankScriptableObjects();
            GenerateParameterScriptableObjects();
            GenerateEventScriptableObjects();
            GenerateSnapshotScriptableObjects();

            EZFMODEditorStatics.DeleteEmptyDirectories(_assetOutputPath);

            AssetDatabase.Refresh();

            // IsGeneratingAssets = false;

            var elapsedTimeMessage = $"{Math.Round(EditorApplication.timeSinceStartup - startTime, 2)}s";
            var generatedAssetsMessage = $"Generated: {_generatedAssets}";
            var modifiedAssetsMessage = $"Modified: {_modifiedAssets}";
            var deletedAssetsMessage = $"Deleted: {_deletedAssets}";

            Debug.Log(
                $"Reconcile finished in {elapsedTimeMessage}: {generatedAssetsMessage} | {modifiedAssetsMessage} | {deletedAssetsMessage}");
        }

        private void GenerateSnapshotScriptableObjects()
        {
            _snapshotScriptableObjects = GenerateScriptableObjects<EZFMODSnapshot, FMODSnapshotData>(_snapshotsData);
        }

        private void GenerateBankScriptableObjects()
        {
            _bankScriptableObjects = GenerateScriptableObjects<EZFMODBank, FMODBankData>(_banksData);
            
            _masterBank = _bankScriptableObjects.Find(bank => bank.IsMasterBank);

            if (_masterBank)
            {
                Debug.Log($"[{nameof(EZFMOD)}] Master bank: {_masterBank.Name}");
            }
            else
            {
                Debug.LogWarning("No master bank found. Please ensure that one bank is marked as the master bank.");
            }
            
            EZFMODSettings.SetMasterBank(_masterBank);
        }
        
        private void GenerateEventScriptableObjects()
        {
            _eventScriptableObjects = GenerateScriptableObjects<EZFMODEvent, FMODEventData>(_eventsData);
            
            // Assign bank objects to events by GUID
            // Assign parameter objects to events by GUID
            foreach (var ezfmodEvent in _eventScriptableObjects)
            {
                var bankObjects = _bankScriptableObjects.Where(bank => ezfmodEvent.BankGUIDs.Contains(bank.GUID)).ToArray();
                ezfmodEvent.SetBankObjects(bankObjects);
                
                // If the event has no parameters, we can skip this step
                if (ezfmodEvent.ParameterGUIDs == null || ezfmodEvent.ParameterGUIDs.Length == 0) continue;
                
                var parameterObjects = _parameterScriptableObjects.Where(parameter => ezfmodEvent.ParameterGUIDs.Contains(parameter.GUID)).ToArray();
                ezfmodEvent.SetParameterObjects(parameterObjects);
            }
        }
        
        private void GenerateParameterScriptableObjects()
        {
            _parameterScriptableObjects = GenerateScriptableObjects<EZFMODParameter, FMODParameterData>(_parametersData);
            
            // Generate parameter value scriptable objects as children of parameter scriptable objects
            foreach (var parameter in _parameterScriptableObjects)
            {
                GenerateParameterValueScriptableObjects(parameter);
                RemoveInvalidSubobjects(parameter);
            }
        }

        private void GenerateParameterValueScriptableObjects(EZFMODParameter parameter)
        {
            if (parameter.Type == ParameterType.Continuous) return;

            var existingChildParameterValues = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(parameter))
                .OfType<EZFMODParameterValue>().ToList();

            var assetsToRemove = new List<EZFMODParameterValue>();
            var assetsToRename = new List<EZFMODParameterValue>();
            var assetsToInitialize = new List<EZFMODParameterValue>();

            assetsToRemove.AddRange(existingChildParameterValues.Where(ShouldRemove));
            assetsToRename.AddRange(existingChildParameterValues.Where(ShouldRename));

            foreach (var asset in assetsToRemove)
            {
                EditorUtility.SetDirty(parameter);
                AssetDatabase.RemoveObjectFromAsset(asset);
                UnityEngine.Object.DestroyImmediate(asset, true);
            }

            foreach (var asset in assetsToRename)
            {
                if (parameter.Type == ParameterType.Labeled && (int)asset.Value < parameter.Labels.Length)
                {
                    // We want to change the value to represent the new label, rather than changing the name
                    var labelOnlyName = asset.name.TrimStart(parameter.Name.ToCharArray()).TrimStart('_');
                    
                    if (parameter.Labels.Contains(labelOnlyName))
                    {
                        asset.Init(parameter, Array.IndexOf(parameter.Labels, labelOnlyName));
                    }
                    else // If the name isn't found in the labels, we'll instead rename the asset according to its value
                    {
                        asset.name = $"{parameter.Name}_{parameter.Labels[(int)asset.Value]}";
                    }
                    
                    EditorUtility.SetDirty(asset);
                }
            }

            for (var i = (int)parameter.Min; i <= (int)parameter.Max; i++)
            {
                var existingParamValue = existingChildParameterValues.Find(pv => Mathf.Approximately(pv.Value, i));
                if (existingParamValue)
                {
                    if (ShouldInitialize(existingParamValue, i))
                    {
                        assetsToInitialize.Add(existingParamValue);
                    }
                    continue;
                }

                var newPv = ScriptableObject.CreateInstance<EZFMODParameterValue>();
                newPv.Init(parameter, i);
                AssetDatabase.AddObjectToAsset(newPv, parameter);
            }

            foreach (var asset in assetsToInitialize)
            {
                asset.Init(parameter, asset.Value);
                EditorUtility.SetDirty(asset);
                EditorUtility.SetDirty(parameter);
            }

            if (assetsToRemove.Count > 0 || assetsToRename.Count > 0 || assetsToInitialize.Count > 0)
            {
                Debug.Log($"[{nameof(EZFMOD)}] Updated parameter values for {parameter.Name}");
            }

            AssetDatabase.SaveAssetIfDirty(parameter);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return;

            // Local functions for readability
            bool ShouldRemove(EZFMODParameterValue parameterValue)
            {
                var isDuplicate = existingChildParameterValues.Count(pv => 
                    Mathf.Approximately(pv.Value, parameterValue.Value) && pv.name == parameterValue.name) > 1;

                return parameter.Type switch
                {
                    ParameterType.Discrete => parameterValue.Value < parameter.Min || parameterValue.Value > parameter.Max || isDuplicate,
                    ParameterType.Labeled => parameterValue.Value < 0 || parameterValue.Value >= parameter.Labels.Length || isDuplicate,
                    _ => false
                };
            }

            bool ShouldRename(EZFMODParameterValue parameterValue) => parameter.Type ==
                ParameterType.Labeled && (int)parameterValue.Value < parameter.Labels.Length &&
                parameterValue.name != $"{parameter.Name}_{parameter.Labels[(int)parameterValue.Value]}";

            bool ShouldInitialize(EZFMODParameterValue paramValue, int index) =>
                parameter.Type == ParameterType.Discrete && paramValue.name != $"{parameter.Name}_{index}" ||
                parameter.Type == ParameterType.Labeled && index < parameter.Labels.Length &&
                paramValue.name != $"{parameter.Name}_{parameter.Labels[index]}";
        }
        
        private void RemoveInvalidSubobjects(EZFMODParameter parameter)
        {
            var subobjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(parameter));
            foreach (var subobject in subobjects)
            {
                if (subobject && MonoScript.FromScriptableObject(subobject as ScriptableObject)) continue;
                
                if (!subobject) continue;
                
                Debug.LogWarning($"Removing invalid subobject ({subobject.name}) from {parameter.name}");
                AssetDatabase.RemoveObjectFromAsset(subobject);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(subobject));
                UnityEngine.Object.DestroyImmediate(subobject, true);
                EditorUtility.SetDirty(parameter);
            }
            
            AssetDatabase.SaveAssetIfDirty(parameter);
            AssetDatabase.Refresh();
        }
        
        private List<T> GenerateScriptableObjects<T, TData>(List<TData> dataList) where T : EZFMODAsset where TData : FMODData
        {
            var existingAssetPaths = EZFMODEditorStatics.GetExistingAssetPaths<T>(_assetOutputPath);
            var existingAssets = existingAssetPaths.Select(AssetDatabase.LoadAssetAtPath<T>).ToList();
            
            var newAssetPaths = new HashSet<string>();
            var updatedAssets = new List<T>();

            foreach (var data in dataList)
            {
                var directoryFromStudioPath = Path.Join(_assetOutputPath, data.GetRelativeAssetPath());
                EZFMODEditorStatics.EnsureDirectoryExists(directoryFromStudioPath);

                if (TryGetExistingAssetFromGUID(data.GUID, existingAssets, out var matchingGuidAsset))
                {
                    var existingAssetPath = AssetDatabase.GetAssetPath(matchingGuidAsset);
                    existingAssetPaths.Remove(existingAssetPath);

                    var newAssetPath = Path.Join(directoryFromStudioPath, data.Name + ".asset");
                    if (existingAssetPath != newAssetPath)
                    {
                        AssetDatabase.MoveAsset(existingAssetPath, newAssetPath);
                        AssetDatabase.SaveAssetIfDirty(matchingGuidAsset);
                    }
                }

                var asset = EZFMODEditorStatics.GetOrCreateScriptableObjectAsset<T>(directoryFromStudioPath, data.Name, out var createdNew);
                if (!asset)
                {
                    Debug.LogWarning($"Failed to create asset for {data.Name}");
                    continue;
                }

                updatedAssets.Add(asset);
                newAssetPaths.Add(AssetDatabase.GetAssetPath(asset));

                if (data is IEZFMODComparable<T> comparable && comparable.InitializeIfDifferent(asset))
                {
                    EditorUtility.SetDirty(asset);
                    if (createdNew) _generatedAssets++;
                    else
                    {
                        Debug.Log($"[{nameof(EZFMOD)}] Modified {typeof(T).Name} : {asset.name}");
                        _modifiedAssets++;
                    }
                }
            }

            foreach (var invalidAssetPath in existingAssetPaths.Except(newAssetPaths))
            {
                AssetDatabase.DeleteAsset(invalidAssetPath);
                _deletedAssets++;
            }
            
            return updatedAssets;
        }
        
        private bool TryGetExistingAssetFromGUID<T>(FMOD.GUID guid, List<T> existingAssets, out T matchingAsset) where T : EZFMODAsset
        {
            matchingAsset = existingAssets.Find(a => a.GUID == guid);
            
            return matchingAsset;
        }
    }
}