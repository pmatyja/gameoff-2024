using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using OCSFX.EZFMODEditor.Metadata.Deserialization;
using Debug = UnityEngine.Debug;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public abstract class FMODData
    {
        protected readonly FMODMetadataObject _mainClassObject;
        protected readonly FMODMetadata _parentMetadata;
        
        public string Name { get; protected set; }
        public GUID GUID { get; protected set; }
        public string StudioPath { get; protected set; }
        public string Class => _mainClassObject.Class;

        public FMODMetadataRelationship[] Relationships { get; protected set; }
        public FMODMetadataProperty[] Properties { get; protected set; }

        protected FMODData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata)
        {
            _mainClassObject = mainClassObject;
            _parentMetadata = parentMetadata;

            var nameProperty = _mainClassObject.GetProperty(EZFMODMetadataGlossary.Attribute.NAME);
            if (nameProperty == null)
            {
                // HANDLE AUDIO FILES
                if (_mainClassObject.Class == EZFMODMetadataGlossary.Class.AUDIO_FILE)
                {
                    var assetPath = _mainClassObject.GetProperty(EZFMODMetadataGlossary.Property.ASSET_PATH).Values[0];
                    Name = assetPath.Split('/').Last();
                }

                else if (_mainClassObject.Class == EZFMODMetadataGlossary.Class.SPATIALISER_EFFECT)
                {
                    // Bespoke instances of SpatialiserEffect do not have a name property. We must assign the default name.
                    Name = $"Unique {EZFMODMetadataGlossary.Class.SPATIALISER_EFFECT} Instance";
                }

                // HANDLE MASTER FOLDERS
                else if (_mainClassObject.Class.Contains(EZFMODMetadataGlossary.Class.FOLDER) &&
                         _mainClassObject.Class.StartsWith(EZFMODMetadataGlossary.Class.MASTER))
                {
                    Name = _mainClassObject.Class;
                }
                else if (_mainClassObject.Class == EZFMODMetadataGlossary.Class.MASTER_SNAPSHOT_GROUP)
                {
                    Name = _mainClassObject.Class;
                }
                else
                {
                    throw new Exception(
                        $"FMOD Data object ({_mainClassObject.Class}) does not have a {EZFMODMetadataGlossary.Attribute.NAME} property. " +
                        "This should only happen for master folder/group classes: " +
                        $"{EZFMODMetadataGlossary.Class.MASTER_BANK_FOLDER}, " +
                        $"{EZFMODMetadataGlossary.Class.MASTER_EVENT_FOLDER}, " +
                        $"{EZFMODMetadataGlossary.Class.MASTER_PARAMETER_PRESET_FOLDER}, " +
                        $"{EZFMODMetadataGlossary.Class.MASTER_SNAPSHOT_GROUP}");
                }
            }
            else
            {
                Name = nameProperty.Values[0];
            }

            GUID = GUID.Parse(_mainClassObject.Id);
            Relationships = _mainClassObject.Relationships.ToArray();
            Properties = _mainClassObject.Properties.ToArray();
        }

        public virtual bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !GUID.IsNull;
        }

        public string GetRelativeAssetPath()
        {
            if (string.IsNullOrEmpty(StudioPath))
            {
                Debug.LogError($"Failed to get relative asset path for {Name}. Studio Path is empty.");
                return string.Empty;
            }

            var directoryFromStudioPath = StudioPath.Replace(":", "");
            var splitPath = directoryFromStudioPath.Split('/').ToList();
            splitPath.RemoveAt(splitPath.Count - 1); // remove the name of the object. We only want the directory path

            return string.Join("/", splitPath);
        }

        public bool TryAssignStudioPath(List<FMODFolderData> folderDataList)
        {
            var builtPath = BuildStudioPath(folderDataList);
            if (string.IsNullOrEmpty(builtPath))
            {
                Debug.LogWarning($"Failed to assign Studio Path to {Name}");
                return false;
            }

            StudioPath = builtPath;
            return true;
        }

        private string BuildStudioPath(List<FMODFolderData> folderDataList)
        {
            var folderNameStack = new Stack<string>();
            folderNameStack.Push(Name);

            if (this is not IFMODFolder folderItem) return string.Join("/", folderNameStack);

            var currentFolderGUID = folderItem.FolderGUID;
            while (!currentFolderGUID.IsNull)
            {
                var currentFolderData = folderDataList.Find(folder => folder.GUID == currentFolderGUID);
                if (currentFolderData == null)
                {
                    Debug.LogWarning($"Failed to find folder with GUID: {currentFolderGUID}. It may be a root folder.");
                    break;
                }

                if (currentFolderData.TryGetMasterFolderName(out var masterFolderName))
                {
                    folderNameStack.Push(masterFolderName);
                    break;
                }

                folderNameStack.Push(currentFolderData.Name);
                currentFolderGUID = currentFolderData.FolderGUID;
            }

            return string.Join("/", folderNameStack);
        }
    }
}