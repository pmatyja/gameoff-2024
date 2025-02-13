using FMOD;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMODEditor.Metadata.Deserialization;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODBankData : FMODData, IFMODFolder, IEZFMODComparable<EZFMODBank>
    {
        public bool IsMasterBank { get; }
        public GUID FolderGUID { get; }
        
        public FMODBankData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata) : base(mainClassObject,
            parentMetadata)
        {
            FolderGUID = GUID.Parse(GetFolderRelationship());

            var masterBankProperty = _mainClassObject.GetProperty(EZFMODMetadataGlossary.Property.IS_MASTER_BANK);
            IsMasterBank = masterBankProperty != null && masterBankProperty.Values[0] == "true";
        }

        public bool HasSameDataAs(EZFMODBank ezFmodAsset)
        {
            var isDifferent = false;
            isDifferent |= Name != ezFmodAsset.Name;
            isDifferent |= StudioPath != ezFmodAsset.StudioPath;
            isDifferent |= GUID != ezFmodAsset.GUID;
            isDifferent |= IsMasterBank != ezFmodAsset.IsMasterBank;

            return !isDifferent;
        }

        /**
         * <summary>
         *     Initializes the EZFMODBank if the data is different from the current asset.
         * </summary>
         * <param name="ezFmodAsset">The EZFMODBank to compare against</param>
         * <returns>True if the data was different and the asset was updated, false if the data was the same.</returns>
         */
        public bool InitializeIfDifferent(EZFMODBank ezFmodAsset)
        {
            var isDifferent = !HasSameDataAs(ezFmodAsset);

            if (isDifferent) ezFmodAsset.Init(Name, StudioPath, GUID, IsMasterBank);

            return isDifferent;
        }

        public string GetFolderRelationship()
        {
            return _mainClassObject.GetRelationship(EZFMODMetadataGlossary.Relationship.FOLDER).Destinations[0];
        }
    }
}