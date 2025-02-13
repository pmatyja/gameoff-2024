using System;
using FMOD;
using OCSFX.EZFMODEditor.Metadata.Deserialization;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODFolderData : FMODData, IFMODFolder
    {
        public bool IsMasterFolder { get; }
        public GUID FolderGUID { get; }
        
        public FMODFolderData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata) : base(mainClassObject,
            parentMetadata)
        {
            var folderRelationship = GetFolderRelationship();

            if (string.IsNullOrEmpty(folderRelationship))
            {
                // This should only be the case a Master folder
                if (!_mainClassObject.Class.StartsWith(EZFMODMetadataGlossary.Class.MASTER))
                    throw new Exception(
                        $"FMOD Folder Data object ({_mainClassObject.Class}) does not have a folder relationship.");

                FolderGUID = new GUID();
                IsMasterFolder = true;
                return;
            }

            FolderGUID = GUID.Parse(GetFolderRelationship());
        }

        public string GetFolderRelationship()
        {
            var relationship = _mainClassObject.GetRelationship(EZFMODMetadataGlossary.Relationship.FOLDER);
            return relationship == null ? string.Empty : relationship.Destinations[0];
        }

        public bool TryGetMasterFolderName(out string masterFolderName)
        {
            masterFolderName = string.Empty;

            if (!IsMasterFolder) return false;

            masterFolderName = string.Empty;
            switch (Class)
            {
                case EZFMODMetadataGlossary.Class.MASTER_BANK_FOLDER:
                    masterFolderName = EZFMODMetadataGlossary.StudioPathRoot.BANK;
                    break;
                case EZFMODMetadataGlossary.Class.MASTER_EVENT_FOLDER:
                    masterFolderName = EZFMODMetadataGlossary.StudioPathRoot.EVENT;
                    break;
                case EZFMODMetadataGlossary.Class.MASTER_PARAMETER_PRESET_FOLDER:
                    masterFolderName = EZFMODMetadataGlossary.StudioPathRoot.PARAMETER;
                    break;
            }

            return masterFolderName != string.Empty;
        }
    }
}