using System;
using FMOD;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility;
using OCSFX.EZFMODEditor.Metadata.Deserialization;
using static OCSFX.EZFMODEditor.Metadata.Types.FMODMetadataUtils;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODEventData : FMODData, IFMODFolder, IEZFMODComparable<EZFMODEvent>
    {
        public GUID[] BankGUIDs { get; set; }
        public GUID[] ParameterGUIDs { get; set; }
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public double Length { get; set; }
        public bool Is3D { get; set; }
        public bool IsOneShot { get; }
        
        public FMODSpatializerData SpatializerData { get; set; }
        public GUID FolderGUID { get; }
        
        public FMODEventData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata) : base(mainClassObject,
            parentMetadata)
        {
            FolderGUID = GUID.Parse(GetFolderRelationship());

            var banksRelationship = GetBanksRelationship();
            BankGUIDs = new GUID[banksRelationship.Length];
            for (var i = 0; i < banksRelationship.Length; i++) BankGUIDs[i] = GUID.Parse(banksRelationship[i]);

            // MinDistance = double.TryParse(GetMinDistanceProperty(), out var minDistance) ? minDistance : 0;
            // MaxDistance = double.TryParse(GetMaxDistanceProperty(), out var maxDistance) ? maxDistance : 0;
            // Length = int.TryParse(GetLengthProperty(), out var length) ? length : 0;
            // Is3D = bool.TryParse(GetIs3DProperty(), out var is3D) && is3D;
            IsOneShot = FMODMetadataUtils.GetIsOneShot(parentMetadata);

            // make sure to initialize these to empty arrays if they are null
            BankGUIDs ??= Array.Empty<GUID>();
            ParameterGUIDs ??= Array.Empty<GUID>();
        }

        public bool HasSameDataAs(EZFMODEvent ezFmodAsset)
        {
            var isDifferent = false;

            isDifferent |= Name != ezFmodAsset.Name;
            isDifferent |= StudioPath != ezFmodAsset.StudioPath;
            isDifferent |= GUID != ezFmodAsset.GUID;
            isDifferent |= !BankGUIDs.HasSameContentAs(ezFmodAsset.BankGUIDs);
            isDifferent |= Is3D != ezFmodAsset.Is3D;
            isDifferent |= IsOneShot != ezFmodAsset.IsOneShot;
            isDifferent |= !ParameterGUIDs.HasSameContentAs(ezFmodAsset.ParameterGUIDs);
            isDifferent |= Math.Abs((float)MinDistance - ezFmodAsset.MinDistance) > EZFMOD_EPSILON;
            isDifferent |= Math.Abs((float)MaxDistance - ezFmodAsset.MaxDistance) > EZFMOD_EPSILON;
            isDifferent |= Math.Abs((float)Length - ezFmodAsset.Length) > EZFMOD_EPSILON;

            return !isDifferent;
        }

        /**
         * <summary>
         *     Initializes the EZFMODEvent if the data is different from the current asset.
         * </summary>
         * <param name="ezFmodAsset">The EZFMODEvent to compare against</param>
         * <returns>True if the data was different and the asset was updated, false if the data was the same.</returns>
         */
        public bool InitializeIfDifferent(EZFMODEvent ezFmodAsset)
        {
            var isDifferent = !HasSameDataAs(ezFmodAsset);

            if (isDifferent)
                ezFmodAsset.Init(Name, StudioPath, GUID, BankGUIDs, Is3D, IsOneShot, ParameterGUIDs, (float)MinDistance,
                    (float)MaxDistance, Length);

            return isDifferent;
        }

        public string GetFolderRelationship()
        {
            return _mainClassObject.GetRelationship(EZFMODMetadataGlossary.Relationship.FOLDER).Destinations[0];
        }

        public override bool IsValid()
        {
            return base.IsValid() && BankGUIDs.Length > 0;
        }

        private string[] GetBanksRelationship()
        {
            var relationship = _mainClassObject.GetRelationship(EZFMODMetadataGlossary.Relationship.BANKS);
            return relationship == null ? Array.Empty<string>() : relationship.Destinations.ToArray();
        }
    }
}