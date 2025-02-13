using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMODEditor.Metadata.Types;
using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor.Metadata.Deserialization
{
    internal static class EZFMODMetadataDeserializer
    {
        internal static void DeserializeFMODProjectMetadata()
        {
            var startTime = EditorApplication.timeSinceStartup;
            
            var fmodProjectMetadata = EZFMODProjectEditorUtility.GetFMODProjectMetadata();
            
            Debug.Log($"[{nameof(EZFMOD)}] Deserializing FMOD Project Metadata...");
            
            // First deserialize dependencies for the main objects
            var bankFolderDataList = GetBankFolders(fmodProjectMetadata);
            var parameterFolderDataList = GetParameterFolders(fmodProjectMetadata);
            var eventFolderDataList = GetEventFolders(fmodProjectMetadata);
            var audioFileDataList = GetAudioFiles(fmodProjectMetadata);
            var spatializerPresetDataList = GetSpatializerPresets(fmodProjectMetadata);
            var snapshotGroupDataList = GetSnapshotGroups(fmodProjectMetadata);
            
            // Then deserialize the main objects
            var bankDataList = GetBanks(fmodProjectMetadata, bankFolderDataList);
            var parameterDataList = GetParameters(fmodProjectMetadata, parameterFolderDataList);
            var eventDataList = GetEvents(fmodProjectMetadata, eventFolderDataList, audioFileDataList, spatializerPresetDataList);
            var snapshotDataList = GetSnapshots(fmodProjectMetadata, snapshotGroupDataList);
            
            var elapsedTime = Math.Round(EditorApplication.timeSinceStartup - startTime, 2);
            
            var elapsedTimeMessage = $"{elapsedTime}s";
            var foundBanksMessage = $"Banks: {bankDataList.Count}";
            var foundBankFoldersMessage = $"Bank Folders: {bankFolderDataList.Count}";
            var foundParametersMessage = $"Parameters: {parameterDataList.Count}";
            var foundParameterFoldersMessage = $"Parameter Folders: {parameterFolderDataList.Count}";
            var foundEventsMessage = $"Events: {eventDataList.Count}";
            var foundEventFoldersMessage = $"Event Folders: {eventFolderDataList.Count}";
            var foundAudioFilesMessage = $"Audio Files: {audioFileDataList.Count}";
            var foundSpatializerPresetsMessage = $"Spatializer Presets: {spatializerPresetDataList.Count}";
            var foundSnapshotsMessage = $"Snapshots: {snapshotDataList.Count}";

            var deserializationMessages = new string[]
            {
                foundBanksMessage,
                foundBankFoldersMessage,
                foundParametersMessage,
                foundParameterFoldersMessage,
                foundEventsMessage,
                foundEventFoldersMessage,
                foundAudioFilesMessage,
                foundSpatializerPresetsMessage,
                foundSnapshotsMessage
            };
            
            var combinedDeserializationMessage = string.Join(" | ", deserializationMessages);
            
            Debug.Log($"Deserialization finished in {elapsedTimeMessage}: " +
                        $"{combinedDeserializationMessage}");
            
            var scriptableObjectGenerator = new EZFMODScriptableObjectAssetGenerator(
                EZFMODEditorStatics.ASSET_OUTPUT_FULL_PATH,
                bankDataList,
                parameterDataList,
                eventDataList,
                snapshotDataList
            );
                
            scriptableObjectGenerator.GenerateScriptableObjects();
        }

        private static List<FMODSnapshotGroupData> GetSnapshotGroups(FMODProjectMetadata fmodProjectMetadata)
        {
            var snapshotGroupFileList = new List<FMODSnapshotGroupData>();
            foreach (var metadataFile in fmodProjectMetadata.SnapshotGroupMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);

                var validClasses = new[]
                {
                    EZFMODMetadataGlossary.Class.SNAPSHOT_GROUP, 
                    EZFMODMetadataGlossary.Class.MASTER_SNAPSHOT_GROUP
                };
                
                if (!TryGetSnapshotGroupData(fmodMetadata, validClasses, out var fmodSnapshotGroupData)) continue;
                
                snapshotGroupFileList.Add(fmodSnapshotGroupData);
            }
            
            return snapshotGroupFileList;
        }

        private static List<FMODSnapshotData> GetSnapshots(FMODProjectMetadata fmodProjectMetadata, List<FMODSnapshotGroupData> snapshotGroupDataList)
        {
            var snapshotFileList = new List<FMODSnapshotData>();
            foreach (var metadataFile in fmodProjectMetadata.SnapshotMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);

                if (!TryGetSnapshotData(fmodMetadata, snapshotGroupDataList, out var fmodSnapshotData)) continue;
                
                snapshotFileList.Add(fmodSnapshotData);
            }
            
            return snapshotFileList;
        }

        private static bool TryGetSnapshotData(FMODMetadata fmodMetadata, List<FMODSnapshotGroupData> snapshotGroupDataList, out FMODSnapshotData fmodSnapshotData)
        {
            fmodSnapshotData = null;
            foreach (var fmodMetadataObject in fmodMetadata.Objects)
            {
                if (fmodMetadataObject.Class != EZFMODMetadataGlossary.Class.SNAPSHOT) continue;

                fmodSnapshotData = new FMODSnapshotData(fmodMetadataObject, fmodMetadata);
                
                if (snapshotGroupDataList.Count == 0) continue;
                
                // Search the snapshot groups to see if the GUID for this snapshot is in any of their items
                foreach (var snapshotGroupData in snapshotGroupDataList)
                {
                    if (snapshotGroupData.ItemGUIDs.Contains(fmodSnapshotData.GUID))
                    {
                        fmodSnapshotData.SnapshotGroupData = snapshotGroupData;
                        fmodSnapshotData.ConformStudioPathWithGroup();
                        break;
                    }
                }
                break;
            }

            if (fmodSnapshotData != null && fmodSnapshotData.IsValid()) return true;
            
            var metadataFileName = fmodMetadata.FileName;
            var printName = fmodSnapshotData?.Name ?? metadataFileName;
            Debug.LogWarning($"FMOD Snapshot ({printName}) metadata is not valid.");
                
            return false;
        }

        private static List<FMODSpatializerData> GetSpatializerPresets(FMODProjectMetadata fmodProjectMetadata)
        {
            var spatializerPresetFileList = new List<FMODSpatializerData>();
            foreach (var metadataFile in fmodProjectMetadata.EffectPresetMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);

                if (!TryDeserializeSpatializerData(fmodMetadata, out var fmodSpatializerData)) continue;
                
                spatializerPresetFileList.Add(fmodSpatializerData);
            }
            
            return spatializerPresetFileList;
        }

        private static bool TryDeserializeSpatializerData(FMODMetadata fmodMetadata, out FMODSpatializerData spatializerData)
        {
            spatializerData = null;
            foreach (var fmodMetadataObject in fmodMetadata.Objects)
            {
                if (fmodMetadataObject.Class != EZFMODMetadataGlossary.Class.EFFECT_PRESET) continue;
                
                // Get the index of the EffectPreset class, because the next one should be the Spatializer
                var objectIndex = fmodMetadata.Objects.IndexOf(fmodMetadataObject);
                if (objectIndex + 1 >= fmodMetadata.Objects.Count) continue;
                
                var nextObject = fmodMetadata.Objects[objectIndex + 1];
                if (nextObject.Class != EZFMODMetadataGlossary.Class.SPATIALISER_EFFECT) continue;
                
                spatializerData = new FMODSpatializerData(fmodMetadataObject, fmodMetadata);
                return true;
            }

            return false;
        }

        private static List<FMODAudioFileData> GetAudioFiles(FMODProjectMetadata fmodProjectMetadata)
        {
            var audioFileList = new List<FMODAudioFileData>();
            foreach (var metadataFile in fmodProjectMetadata.AudioFileMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);

                if (!TryDeserializeAudioFileData(fmodMetadata, out var fmodAudioFileData)) continue;
                
                audioFileList.Add(fmodAudioFileData);
            }
            
            return audioFileList;
        }

        private static bool TryDeserializeAudioFileData(FMODMetadata fmodMetadata, out FMODAudioFileData fmodAudioFileData)
        {
            fmodAudioFileData = null;
            foreach (var fmodMetadataObject in fmodMetadata.Objects)
            {
                if (fmodMetadataObject.Class != EZFMODMetadataGlossary.Class.AUDIO_FILE) continue;
                
                fmodAudioFileData = new FMODAudioFileData(fmodMetadataObject, fmodMetadata);
                return true;
            }

            return false;
        }

        private static FMODMetadata DeserializeFile(string xmlFilePath)
        {
            var serializer = new XmlSerializer(typeof(FMODMetadata));
            using var fileStream = new FileStream(xmlFilePath, FileMode.Open);
            var returnMetadata = (FMODMetadata)serializer.Deserialize(fileStream);
            returnMetadata.FileName = Path.GetFileName(xmlFilePath);
            
            EZFMODMetadataValidation.ValidateFMODMetadata(returnMetadata);
            
            return returnMetadata;
        }
        
        private static List<FMODBankData> GetBanks(FMODProjectMetadata fmodProjectMetadata, List<FMODFolderData> folderDataList)
        {
            var bankList = new List<FMODBankData>();
            foreach (var metadataFile in fmodProjectMetadata.BankMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);

                if (!TryGetBankData(fmodMetadata, folderDataList, out var fmodBankData)) continue;
                
                bankList.Add(fmodBankData);
            }

            return bankList;
        }
        
        private static List<FMODParameterData> GetParameters(FMODProjectMetadata fmodProjectMetadata, List<FMODFolderData> folderDataList)
        {
            var parameterList = new List<FMODParameterData>();

            foreach (var parameterMetadataFile in fmodProjectMetadata.ParameterPresetMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(parameterMetadataFile);

                if (!TryGetParameterData(fmodMetadata, folderDataList, out var fmodParameterData)) continue;
                
                parameterList.Add(fmodParameterData);
            }

            return parameterList;
        }

        private static List<FMODEventData> GetEvents(FMODProjectMetadata fmodProjectMetadata, 
            List<FMODFolderData> folderDataList, 
            List<FMODAudioFileData> audioFileDataList,
            List<FMODSpatializerData> spatializerPresetData)
        {
            var eventList = new List<FMODEventData>();

            foreach (var eventMetadataFile in fmodProjectMetadata.EventMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(eventMetadataFile);
                
                // EZTestParseMetadata(fmodMetadata, EZFMODGlossary.Class.EVENT, "sx_int_structure_large_move_begin");

                if (!TryDeserializeEventData(fmodMetadata, folderDataList, audioFileDataList, spatializerPresetData, out var fmodEventData)) continue;

                eventList.Add(fmodEventData);
            }
            
            return eventList;
        }
        
#region Event deserialization
        private static bool TryDeserializeEventData(FMODMetadata metaData, 
            List<FMODFolderData> folderData, 
            List<FMODAudioFileData> audioFilesData, 
            List<FMODSpatializerData> spatializerPresetData,
            out FMODEventData fmodEventData)
        {
            if (!TryGetEventData(metaData, folderData, out fmodEventData)) return false;
            if (!fmodEventData.IsValid()) return false;

            SetEventDistance(fmodEventData, metaData, spatializerPresetData);
            SetEventLength(fmodEventData, metaData, audioFilesData);
            
            return true;
        }

        private static void SetEventLength( FMODEventData fmodEventData, FMODMetadata metaData, List<FMODAudioFileData> audioFilesData)
        {
            // If we can't find a length property in the event, then we need to look for audio file relationships
            if (TryGetLengthProperties(metaData, out var lengthProperties))
            {
                var values = lengthProperties.Select(property => property.Values).ToList();
                var asNumbers = values.SelectMany(value => value).Select(double.Parse).ToList();
                
                // If we find multiple length properties, then we should take the max value
                fmodEventData.Length = asNumbers.Max();
            }
            else
            {
                // Fall back on looking at the audio files referenced in the event to get some length value
                if (TryGetAudioFileRelationships(metaData, out var audioFileRelationships))
                {
                    var audioFileGUIDs = audioFileRelationships
                        .SelectMany(relationship => relationship.GetDestinationGUIDs()).ToArray();
                    
                    var audioFiles = audioFilesData.Where(audioFile => audioFileGUIDs.Contains(audioFile.GUID)).ToList();
                    fmodEventData.Length = audioFiles.Count > 0 ? audioFiles.Max(audioFile => audioFile.Length) : 0;
                }
            }
        }

        private static void SetEventDistance(FMODEventData fmodEventData, FMODMetadata metaData, List<FMODSpatializerData> spatializerPresetData)
        {
            if (TryGetSpatializerObject(metaData, out var spatializerObject))
            {
                fmodEventData.SpatializerData = spatializerObject;
            }
            else if (TryGetProxyEffects(metaData, out var proxyEffectObjects))
            {
                foreach (var proxyEffect in proxyEffectObjects)
                {
                    if (fmodEventData.SpatializerData != null) break;

                    var presetRelationship = proxyEffect.GetRelationship(EZFMODMetadataGlossary.Relationship.PRESET);
                    if (presetRelationship == null) continue;
                    
                    var destinationGUIDs = presetRelationship.GetDestinationGUIDs();
                    foreach (var spatializerPreset in spatializerPresetData)
                    {
                        if (!destinationGUIDs.Contains(spatializerPreset.GUID)) continue;
                        
                        fmodEventData.SpatializerData = spatializerPreset;
                        break;
                    }
                }
            }

            if (fmodEventData.SpatializerData == null) return;
            
            fmodEventData.Is3D = true;
            fmodEventData.MinDistance = fmodEventData.SpatializerData.MinDistance;
            fmodEventData.MaxDistance = fmodEventData.SpatializerData.MaxDistance;
        }

        private static bool TryGetBanksRelationship(FMODMetadata metadata, out FMODMetadataRelationship relationship)
        {
            relationship = null;
            foreach (var metadataObject in metadata.Objects)
            {
                relationship = metadataObject.GetRelationship(EZFMODMetadataGlossary.Relationship.BANKS);
                break;
            }
            
            return relationship != null;
        }

        private static bool TryGetParameterRelationships(FMODMetadata metadata, out List<FMODMetadataRelationship> relationships)
        {
            relationships = new List<FMODMetadataRelationship>();

            foreach (var metadataObject in metadata.Objects)
            {
                if (!TryGetParameterRelationship(metadataObject, out var relationship)) continue;
                relationships.Add(relationship);
            }
            
            return relationships.Count > 0;
        }
        
        private static bool TryGetLengthProperties(FMODMetadata metadata, out List<FMODMetadataProperty> properties)
        {
            properties = new List<FMODMetadataProperty>();
            foreach (var metadataObject in metadata.Objects)
            {
                if (!TryGetLengthProperty(metadataObject, out var lengthProperty)) continue;
                properties.Add(lengthProperty);
            }

            return properties.Count > 0;
        }
        
        private static bool TryGetEventData(FMODMetadata fmodMetadata, List<FMODFolderData> folderData, out FMODEventData fmodEventData)
        {
            fmodEventData = null;
            if (fmodMetadata.Objects.Count == 0) return false;
            
            var mainClassObject = fmodMetadata.Objects.FirstOrDefault(obj => obj.Class == EZFMODMetadataGlossary.Class.EVENT);
            
            if (mainClassObject == null) return false;
            
            fmodEventData = new FMODEventData(mainClassObject, fmodMetadata);
            
            if (TryGetBanksRelationship(fmodMetadata, out var banksRelationship))
            {
                fmodEventData.BankGUIDs = banksRelationship.GetDestinationGUIDs();
            }
            
            if (TryGetParameterRelationships(fmodMetadata, out var parameterRelationships))
            {
                fmodEventData.ParameterGUIDs = parameterRelationships
                    .SelectMany(relationship => relationship.GetDestinationGUIDs()).ToArray();
            }
            
            if (!fmodEventData.IsValid())
            {
                Debug.LogWarning($"FMOD Event ({fmodEventData.Name}) metadata is not valid. " +
                                 $"Most likely it is missing Banks. No {nameof(EZFMODAsset)} will be created for this event.");
            }

            if (!fmodEventData.TryAssignStudioPath(folderData))
            {
                Debug.LogWarning($"Failed to assign Studio Path to FMOD Event ({fmodEventData.Name}).");
            }
            
            return true;
        }

        private static bool TryGetEventData(FMODMetadata fmodMetadata, out FMODEventData fmodEventData)
        {
            fmodEventData = null;
            if (fmodMetadata.Objects.Count == 0) return false;
            
            var fmodMetadataObject = fmodMetadata.Objects.FirstOrDefault(obj => obj.Class == EZFMODMetadataGlossary.Class.EVENT);
            if (fmodMetadataObject == null) return false;
                
            fmodEventData = new FMODEventData(fmodMetadataObject, fmodMetadata);
            if (!fmodEventData.IsValid())
            {
                Debug.LogWarning($"FMOD Event ({fmodEventData.Name}) from {fmodMetadataObject.Class} metadata is not valid. " +
                                 $"Most likely it is missing Banks. Either assign bank(s) or refrain from referencing this asset.");
            }

            return true;
        }
        
        private static bool TryGetParameterRelationship(FMODMetadataObject fmodMetadataObject, out FMODMetadataRelationship relationship)
        {
            // check all "parameter" relationships for their destination GUIDs
            relationship = fmodMetadataObject.GetRelationship(EZFMODMetadataGlossary.Relationship.PARAMETER);

            if (relationship != null) return true;
            
            // Check for ParameterProxy class. If it is, then also check for a "preset" relationship
            if (fmodMetadataObject.Class == EZFMODMetadataGlossary.Class.PARAMETER_PROXY)
            {
                relationship = fmodMetadataObject.GetRelationship(EZFMODMetadataGlossary.Relationship.PRESET);
            }

            return relationship != null;
        }
        
        private static bool TryGetLengthProperty(FMODMetadataObject fmodMetadataObject, out FMODMetadataProperty property)
        {
            property = null;
            
            var isValidClass = fmodMetadataObject.Class is
                EZFMODMetadataGlossary.Class.SINGLE_SOUND
                or EZFMODMetadataGlossary.Class.MULTI_SOUND;
            
            if (!isValidClass) return false;
            
            property = fmodMetadataObject.GetProperty(EZFMODMetadataGlossary.Property.LENGTH);
            if (property == null)
            {
                // TODO: Deserialize AudioFile xml files so we can find the length of the audio files referenced in the event
            }
            return property != null;
        }
        
        private static bool TryGetAudioFileRelationships(FMODMetadata metadata, out List<FMODMetadataRelationship> relationships)
        {
            relationships = null;
            if (metadata.Objects.Count == 0) return false;
            
            relationships = new List<FMODMetadataRelationship>();
            foreach (var metadataObject in metadata.Objects)
            {
                var relationship = metadataObject.GetRelationship(EZFMODMetadataGlossary.Relationship.AUDIO_FILE);
                if (relationship == null) continue;
                relationships.Add(relationship);
            }
            
            return relationships.Count > 0;
        }
        
        private static bool TryGetSpatializerObject(FMODMetadata metadata, out FMODSpatializerData spatializerData)
        {
            // If this returns true, then it means the instance of Spatializer is a unique one and not a preset.
            // That makes things easier so we don't have to go search for the preset.
            spatializerData = null;
            if (metadata.Objects.Count == 0) return false;
            
            var fmodMetadataObject = metadata.Objects.FirstOrDefault(obj => obj.Class == EZFMODMetadataGlossary.Class.SPATIALISER_EFFECT);
            if (fmodMetadataObject == null) return false;
                
            spatializerData = new FMODSpatializerData(fmodMetadataObject, metadata);
            return true;
        }
        
        private static bool TryGetProxyEffect(FMODMetadataObject fmodMetadataObject, out FMODMetadataObject proxyEffectObject)
        {
            proxyEffectObject = null;
            if (fmodMetadataObject.Class != EZFMODMetadataGlossary.Class.PROXY_EFFECT) return false;
                
            proxyEffectObject = fmodMetadataObject;
            return true;
        }
        
        private static bool TryGetProxyEffects(FMODMetadata metadata, out List<FMODMetadataObject> proxyEffectObjects)
        {
            proxyEffectObjects = null;
            if (metadata.Objects.Count == 0) return false;
            
            proxyEffectObjects = new List<FMODMetadataObject>();
            foreach (var metadataObject in metadata.Objects)
            {
                if (metadataObject.Class != EZFMODMetadataGlossary.Class.PROXY_EFFECT) continue;
                
                proxyEffectObjects.Add(metadataObject);
            }

            return proxyEffectObjects.Count > 0;
        }
#endregion

        private static bool TryGetParameterData(FMODMetadata metadata, List<FMODFolderData> folderData, out FMODParameterData fmodParameterData)
        {
            fmodParameterData = null;
            if (metadata.Objects.Count == 0) return false;
            
            var classObject = metadata.Objects.FirstOrDefault(obj => obj.Class == EZFMODMetadataGlossary.Class.PARAMETER_PRESET);
            var gameParamObject = metadata.Objects.FirstOrDefault(obj => obj.Class == EZFMODMetadataGlossary.Class.GAME_PARAMETER);
                    
            if (classObject == null || gameParamObject == null)
            {
                Debug.LogError("Failed to find Class and Game Parameter objects.");
                return false;
            }
                    
            fmodParameterData = new FMODParameterData(classObject, gameParamObject, metadata);
            if (!fmodParameterData.IsValid())
            {
                Debug.LogWarning($"FMOD Parameter ({fmodParameterData.Name}) metadata is not valid. " +
                                 $"Most likely it is missing Banks and therefore can be ignored.");
            }

            if (!fmodParameterData.TryAssignStudioPath(folderData))
            {
                Debug.LogWarning($"Failed to assign Studio Path to FMOD Parameter ({fmodParameterData.Name}).");
            }
            
            return true;
        }

        private static bool TryGetBankData(FMODMetadata metadata, List<FMODFolderData> folderData, out FMODBankData fmodBankData)
        {
            fmodBankData = null;
            if (metadata.Objects.Count == 0) return false;
            foreach (var fmodMetadataObject in metadata.Objects)
            {
                if (fmodMetadataObject.Class != EZFMODMetadataGlossary.Class.BANK) continue;
                
                fmodBankData = new FMODBankData(fmodMetadataObject, metadata);
                break;
            }

            if (fmodBankData == null || !fmodBankData.IsValid()) return false;
            
            if (!fmodBankData.TryAssignStudioPath(folderData))
            {
                Debug.LogWarning($"Failed to assign Studio Path to FMOD Bank ({fmodBankData.Name}).");
            }
                
            return true;
        }


#region Folder deserialization
        private static List<FMODFolderData> GetBankFolders(FMODProjectMetadata fmodProjectMetadata)
        {
            var bankFolderList = new List<FMODFolderData>();
            foreach (var metadataFile in fmodProjectMetadata.BankFolderMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);
                var bankFolderClasses = new[] {EZFMODMetadataGlossary.Class.BANK_FOLDER, EZFMODMetadataGlossary.Class.MASTER_BANK_FOLDER};

                if (TryGetFolderData(fmodMetadata, bankFolderClasses, out var fmodBankFolderData))
                {
                    bankFolderList.Add(fmodBankFolderData);
                }
            }
            
            return bankFolderList;
        }

        private static List<FMODFolderData> GetEventFolders(FMODProjectMetadata fmodProjectMetadata)
        {
            var eventFolderList = new List<FMODFolderData>();
            
            foreach (var metadataFile in fmodProjectMetadata.EventFolderMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);
                var eventFolderClasses = new[] {EZFMODMetadataGlossary.Class.EVENT_FOLDER, EZFMODMetadataGlossary.Class.MASTER_EVENT_FOLDER};

                if (TryGetFolderData(fmodMetadata, eventFolderClasses, out var fmodEventFolderData))
                {
                    eventFolderList.Add(fmodEventFolderData);
                }
            }
            
            return eventFolderList;
        }
        
        private static List<FMODFolderData> GetParameterFolders(FMODProjectMetadata fmodProjectMetadata)
        {
            var parameterPresetFolderList = new List<FMODFolderData>();
            
            foreach (var metadataFile in fmodProjectMetadata.ParameterPresetFolderMetadataFiles)
            {
                var fmodMetadata = DeserializeFile(metadataFile);
                var parameterFolderClasses = new[] {EZFMODMetadataGlossary.Class.PARAMETER_PRESET_FOLDER, EZFMODMetadataGlossary.Class.MASTER_PARAMETER_PRESET_FOLDER};

                if (TryGetFolderData(fmodMetadata, parameterFolderClasses, out var fmodParameterFolderData))
                {
                    parameterPresetFolderList.Add(fmodParameterFolderData);
                }
            }
            
            return parameterPresetFolderList;
        }
        
        private static bool TryGetFolderData(FMODMetadata metadata, string[] validClasses, out FMODFolderData folderData)
        {
            folderData = null;
            
            if (metadata.Objects.Count == 0) return false;
            foreach (var metadataObject in metadata.Objects)
            {
                if (!validClasses.Contains(metadataObject.Class)) continue;
                
                folderData = new FMODFolderData(metadataObject, metadata);
                break;
            }
            
            return folderData != null && folderData.IsValid();
        }
        
        private static bool TryGetSnapshotGroupData(FMODMetadata metadata, string[] validClasses, out FMODSnapshotGroupData snapshotGroupData)
        {
            snapshotGroupData = null;
            
            if (metadata.Objects.Count == 0) return false;
            foreach (var metadataObject in metadata.Objects)
            {
                if (!validClasses.Contains(metadataObject.Class)) continue;
                
                snapshotGroupData = new FMODSnapshotGroupData(metadataObject, metadata);
                break;
            }
            
            return snapshotGroupData != null && snapshotGroupData.IsValid();
        }
#endregion
    }
}