using System;
using FMOD;
using OCSFX.EZFMODEditor.Metadata.Deserialization;
using Debug = UnityEngine.Debug;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODSnapshotGroupData : FMODData
    {
        public GUID[] ItemGUIDs { get; }
        
        public FMODSnapshotGroupData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata) : base(
            mainClassObject, parentMetadata)
        {
            var itemsRelationship = GetItemsRelationship();
            if (itemsRelationship == null)
            {
                Debug.LogError($"No {EZFMODMetadataGlossary.Relationship.ITEMS} relationship found for {Name}");
                ItemGUIDs = Array.Empty<GUID>();
                return;
            }

            ItemGUIDs = itemsRelationship.GetDestinationGUIDs();
        }

        public bool IsMasterSnapshotGroup()
        {
            return _mainClassObject.Class == EZFMODMetadataGlossary.Class.MASTER_SNAPSHOT_GROUP;
        }

        private FMODMetadataRelationship GetItemsRelationship()
        {
            return _mainClassObject.GetRelationship(EZFMODMetadataGlossary.Relationship.ITEMS);
        }
    }
}