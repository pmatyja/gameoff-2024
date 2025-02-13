using FMOD;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMODEditor.Metadata.Deserialization;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODSnapshotData : FMODData, IEZFMODComparable<EZFMODSnapshot>
    {
        public FMODSnapshotGroupData SnapshotGroupData { get; set; }
        
        public FMODSnapshotData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata) : base(mainClassObject,
            parentMetadata)
        {
            Name = _mainClassObject.GetProperty(EZFMODMetadataGlossary.Attribute.NAME).Values[0];
            GUID = GUID.Parse(_mainClassObject.Id);
            StudioPath = EZFMODMetadataGlossary.StudioPathRoot.SNAPSHOT + "/" + Name;
        }

        public bool HasSameDataAs(EZFMODSnapshot ezFmodAsset)
        {
            var isDifferent = false;

            isDifferent |= Name != ezFmodAsset.Name;
            isDifferent |= StudioPath != ezFmodAsset.StudioPath;
            isDifferent |= GUID != ezFmodAsset.GUID;

            return !isDifferent;
        }

        public bool InitializeIfDifferent(EZFMODSnapshot ezFmodAsset)
        {
            var isDifferent = !HasSameDataAs(ezFmodAsset);

            if (isDifferent) ezFmodAsset.Init(Name, StudioPath, GUID);

            return isDifferent;
        }

        public void ConformStudioPathWithGroup()
        {
            if (SnapshotGroupData == null || SnapshotGroupData.IsMasterSnapshotGroup())
                // This should only happen if the Snapshot is not part of a Snapshot Group
                return;

            StudioPath =
                EZFMODMetadataGlossary.StudioPathRoot.SNAPSHOT + "/" + SnapshotGroupData.Name + "/" + Name;
        }

        public override bool IsValid()
        {
            return base.IsValid() && !string.IsNullOrEmpty(StudioPath);
        }
    }
}