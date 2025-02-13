using System;
using FMOD;
using FMOD.Studio;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility;
using OCSFX.EZFMODEditor.Metadata.Deserialization;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODParameterData : FMODData, IFMODFolder, IEZFMODComparable<EZFMODParameter>
    {
        private readonly FMODMetadataObject _gameParameterClassObject;
        
        public int Type { get; }
        public bool IsGlobal { get; }
        public double Minimum { get; }
        public double Maximum { get; }
        public double InitialValue { get; }
        public string[] Labels { get; }

        private PARAMETER_ID ID { get; }
        public GUID FolderGUID { get; }

        public FMODParameterData(FMODMetadataObject mainClassObject, FMODMetadataObject gameParameterClassObject,
            FMODMetadata parentMetadata) : base(mainClassObject, parentMetadata)
        {
            _gameParameterClassObject = gameParameterClassObject;

            // Parameters are a unique case. The outward-facing GUID we really need is within the GameParameter object, not the ParameterPreset object
            // This differs from the other classes, where the GUID is in the class object id
            GUID = GUID.Parse(_gameParameterClassObject.Id);

            FolderGUID = GUID.Parse(GetFolderRelationship());

            Type = int.TryParse(GetParameterTypeProperty(), out var type) ? type : 0;

            IsGlobal = bool.TryParse(GetIsGlobalProperty(), out var isGlobal) && isGlobal;

            Minimum = double.TryParse(GetMinimumProperty(), out var minimum) ? minimum : 0;

            Maximum = GetMaximum();

            InitialValue = double.TryParse(GetInitialValueProperty(), out var initialValue) ? initialValue : Minimum;

            Labels = GetLabelsProperty();

            ID = GetParameterID();
        }

        public bool HasSameDataAs(EZFMODParameter ezFmodAsset)
        {
            var isDifferent = false;
            isDifferent |= Name != ezFmodAsset.Name;
            isDifferent |= StudioPath != ezFmodAsset.StudioPath;
            isDifferent |= GUID != ezFmodAsset.GUID;
            isDifferent |= Type != (int)ezFmodAsset.Type;
            isDifferent |= IsGlobal != ezFmodAsset.IsGlobal;
            isDifferent |= Math.Abs((float)Minimum - ezFmodAsset.Min) > float.Epsilon;
            isDifferent |= Math.Abs((float)Maximum - ezFmodAsset.Max) > float.Epsilon;
            isDifferent |= Math.Abs((float)InitialValue - ezFmodAsset.Default) > float.Epsilon;
            isDifferent |= !Labels.HasSameContentAs(ezFmodAsset.Labels);

            return !isDifferent;
        }

        /**
         * <summary>
         *     Initializes the EZFMODParameter if the data is different from the current asset.
         * </summary>
         * <param name="ezFmodAsset">The EZFMODParameter to compare against</param>
         * <returns>True if the data was different and the asset was updated, false if the data was the same.</returns>
         */
        public bool InitializeIfDifferent(EZFMODParameter ezFmodAsset)
        {
            var isDifferent = !HasSameDataAs(ezFmodAsset);

            if (isDifferent)
                ezFmodAsset.Init(Name, GUID, StudioPath, Minimum, Maximum, InitialValue, ID, Type, IsGlobal, Labels);

            return isDifferent;
        }

        public string GetFolderRelationship()
        {
            var relationship = _mainClassObject.GetRelationship(EZFMODMetadataGlossary.Relationship.FOLDER);
            return relationship == null ? string.Empty : relationship.Destinations[0];
        }

        public string GetParameterTypeProperty()
        {
            var property = _gameParameterClassObject.GetProperty(EZFMODMetadataGlossary.Property.PARAMETER_TYPE);
            return property == null ? string.Empty : property.Values[0];
        }

        public string GetIsGlobalProperty()
        {
            var property = _gameParameterClassObject.GetProperty(EZFMODMetadataGlossary.Property.IS_GLOBAL);
            return property == null ? string.Empty : property.Values[0];
        }

        public string GetMinimumProperty()
        {
            var property = _gameParameterClassObject.GetProperty(EZFMODMetadataGlossary.Property.MINIMUM);
            return property == null ? string.Empty : property.Values[0];
        }

        public string GetMaximumProperty()
        {
            var property = _gameParameterClassObject.GetProperty(EZFMODMetadataGlossary.Property.MAXIMUM);
            return property == null ? string.Empty : property.Values[0];
        }

        public string GetInitialValueProperty()
        {
            var property = _gameParameterClassObject.GetProperty(EZFMODMetadataGlossary.Property.INITIAL_VALUE);
            return property == null ? string.Empty : property.Values[0];
        }

        public string[] GetLabelsProperty()
        {
            var property = _gameParameterClassObject.GetProperty(EZFMODMetadataGlossary.Property.LABELS);
            return property == null ? Array.Empty<string>() : property.Values.ToArray();
        }

        private double GetMaximum()
        {
            // For some reason in the metadata, when parameter is type 1 or 2 (discrete or labeled), the maximum value is one higher than it should be.
            // This is a workaround to fix that...

            var property = _gameParameterClassObject.GetProperty(EZFMODMetadataGlossary.Property.MAXIMUM);
            if (property == null) return 1;

            var maximum = double.Parse(property.Values[0]);
            return Type > 0 ? maximum - 1 : maximum;
        }

        private PARAMETER_ID GetParameterID()
        {
            return EZFMODEditorStatics.GetParameterIDFromGUID(GUID, IsGlobal);
        }
    }
}