using System.Collections.Generic;
using System.Xml.Serialization;
using FMOD;

namespace OCSFX.EZFMODEditor.Metadata.Deserialization
{
    // DESERIALIZATION CLASSES

    [XmlRoot(EZFMODMetadataGlossary.Element.OBJECTS)]
    public class FMODMetadata
    {
        public string FileName { get; set; }
        
        [XmlElement(EZFMODMetadataGlossary.Element.OBJECT)]
        public List<FMODMetadataObject> Objects { get; set; }
    }

    public class FMODMetadataObject
    {
        [XmlAttribute(EZFMODMetadataGlossary.Attribute.CLASS)]
        public string Class { get; set; }

        [XmlAttribute(EZFMODMetadataGlossary.Attribute.ID)]
        public string Id { get; set; }

        [XmlElement(EZFMODMetadataGlossary.Element.PROPERTY)]
        public List<FMODMetadataProperty> Properties { get; set; }

        [XmlElement(EZFMODMetadataGlossary.Element.RELATIONSHIP)]
        public List<FMODMetadataRelationship> Relationships { get; set; }
        
        public FMODMetadataProperty GetProperty(string propertyName)
        {
            return Properties.Find(property => property.Name == propertyName);
        }
        
        public FMODMetadataRelationship GetRelationship(string relationshipName)
        {
            return Relationships.Find(relationship => relationship.Name == relationshipName);
        }
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Class) && !string.IsNullOrEmpty(Id);
        }
        
        public bool IsMasterFolder()
        {
            return Class is 
                EZFMODMetadataGlossary.Class.MASTER_BANK_FOLDER 
                or EZFMODMetadataGlossary.Class.MASTER_EVENT_FOLDER 
                or EZFMODMetadataGlossary.Class.MASTER_PARAMETER_PRESET_FOLDER;
        }
    }

    public class FMODMetadataProperty
    {
        [XmlAttribute(EZFMODMetadataGlossary.Attribute.NAME)]
        public string Name { get; set; }

        [XmlElement(EZFMODMetadataGlossary.Element.VALUE)]
        public List<string> Values { get; set; }
    }

    public class FMODMetadataRelationship
    {
        [XmlAttribute(EZFMODMetadataGlossary.Attribute.NAME)]
        public string Name { get; set; }

        [XmlElement(EZFMODMetadataGlossary.Element.DESTINATION)]
        public List<string> Destinations { get; set; }
        
        public GUID[] GetDestinationGUIDs()
        {
            var guids = new GUID[Destinations.Count];
            for (var i = 0; i < Destinations.Count; i++)
            {
                guids[i] = GUID.Parse(Destinations[i]);
            }

            return guids;
        }
    }

    public struct FMODProjectMetadata
    {
        public string[] EventMetadataFiles { get; private set; }
        public string[] EventFolderMetadataFiles { get; private set; }
        public string[] BankMetadataFiles { get; private set; }
        public string[] BankFolderMetadataFiles { get; private set; }
        public string[] ParameterPresetMetadataFiles { get; private set; }
        public string[] ParameterPresetFolderMetadataFiles { get; private set; }
        public string[] AudioFileMetadataFiles { get; private set; }
        public string[] EffectPresetMetadataFiles { get; private set; }
        public string[] SnapshotMetadataFiles { get; private set; }
        public string[] SnapshotGroupMetadataFiles { get; private set; }
        
        // TODO: Add effect preset metadata files so we can get distance values used by spatializers
        // To make these connections, we need to parse event metadata to look either for "spatialiser" or a proxy effect property somewhere in the file
        // public string[] EffectPresetMetadataFiles { get; private set; }
        
        public static readonly FMODProjectMetadata Empty = new FMODProjectMetadata();

        public bool IsValid()
        {
            return EventMetadataFiles != null 
                   && EventFolderMetadataFiles != null
                   && BankMetadataFiles != null 
                   && BankFolderMetadataFiles != null
                   && ParameterPresetMetadataFiles != null
                   && ParameterPresetFolderMetadataFiles != null
                   && AudioFileMetadataFiles != null
                   && EffectPresetMetadataFiles != null
                   && SnapshotMetadataFiles != null
                   && SnapshotGroupMetadataFiles != null;
        }

        public class Builder
        {
            private FMODProjectMetadata _fmodProjectMetadata;

            public Builder SetEventMetadataFiles(string[] eventMetadataFiles)
            {
                _fmodProjectMetadata.EventMetadataFiles = eventMetadataFiles;
                return this;
            }
            
            public Builder SetEventFolderMetadataFiles(string[] eventFolderMetadataFiles)
            {
                _fmodProjectMetadata.EventFolderMetadataFiles = eventFolderMetadataFiles;
                return this;
            }
            
            public Builder SetBankMetadataFiles(string[] bankMetadataFiles)
            {
                _fmodProjectMetadata.BankMetadataFiles = bankMetadataFiles;
                return this;
            }
            
            public Builder SetBankFolderMetadataFiles(string[] bankFolderMetadataFiles)
            {
                _fmodProjectMetadata.BankFolderMetadataFiles = bankFolderMetadataFiles;
                return this;
            }
            
            public Builder SetParameterPresetMetadataFiles(string[] parameterPresetMetadataFiles)
            {
                _fmodProjectMetadata.ParameterPresetMetadataFiles = parameterPresetMetadataFiles;
                return this;
            }
            
            public Builder SetParameterPresetFolderMetadataFiles(string[] parameterPresetFolderMetadataFiles)
            {
                _fmodProjectMetadata.ParameterPresetFolderMetadataFiles = parameterPresetFolderMetadataFiles;
                return this;
            }
            
            public Builder SetAudioFileMetadataFiles(string[] audioFileMetadataFiles)
            {
                _fmodProjectMetadata.AudioFileMetadataFiles = audioFileMetadataFiles;
                return this;
            }
            
            public Builder SetEffectPresetMetadataFiles(string[] effectPresetMetadataFiles)
            {
                _fmodProjectMetadata.EffectPresetMetadataFiles = effectPresetMetadataFiles;
                return this;
            }
            
            public Builder SetSnapshotMetadataFiles(string[] snapshotMetadataFiles)
            {
                _fmodProjectMetadata.SnapshotMetadataFiles = snapshotMetadataFiles;
                return this;
            }
            
            public Builder SetSnapshotGroupMetadataFiles(string[] snapshotGroupMetadataFiles)
            {
                _fmodProjectMetadata.SnapshotGroupMetadataFiles = snapshotGroupMetadataFiles;
                return this;
            }
            
            public FMODProjectMetadata Build() => _fmodProjectMetadata;
        }
    }
}