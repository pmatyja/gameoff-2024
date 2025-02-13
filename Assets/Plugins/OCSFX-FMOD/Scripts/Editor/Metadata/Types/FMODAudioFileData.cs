using FMOD;
using OCSFX.EZFMODEditor.Metadata.Deserialization;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODAudioFileData : FMODData, IFMODFolder, IFMODDuration
    {
        public GUID FolderGUID { get; }
        public double Length { get; }
        
        public FMODAudioFileData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata) : base(
            mainClassObject, parentMetadata)
        {
            FolderGUID = GUID.Parse(GetFolderRelationship());
            Length = double.TryParse(GetLengthProperty(), out var length) ? length : 0;
        }

        public string GetLengthProperty()
        {
            return _mainClassObject.GetProperty(EZFMODMetadataGlossary.Property.LENGTH).Values[0];
        }

        public string GetFolderRelationship()
        {
            return _mainClassObject.GetRelationship(EZFMODMetadataGlossary.Relationship.MASTER_ASSET_FOLDER).Destinations[0];
        }
    }
}