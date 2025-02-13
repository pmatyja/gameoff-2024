using FMOD;
using OCSFX.EZFMODEditor.Metadata.Deserialization;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public class FMODSpatializerData : FMODData
    {
        private readonly FMODMetadataObject _spatialiserEffectClassObject;
        
        public double MinDistance { get; } = 1.0; // Default FMOD Spatializer Min Distance
        public double MaxDistance { get; } = 20.0; // Default FMOD Spatializer Max Distance


        public FMODSpatializerData(FMODMetadataObject mainClassObject, FMODMetadata parentMetadata) : base(
            mainClassObject, parentMetadata)
        {
            _spatialiserEffectClassObject =
                parentMetadata.Objects.Find(obj => obj.Class == EZFMODMetadataGlossary.Class.SPATIALISER_EFFECT);
            GUID = GUID.Parse(mainClassObject.Id);

            var minDistanceProperty = GetMinDistanceProperty();
            var maxDistanceProperty = GetMaxDistanceProperty();

            if (!string.IsNullOrEmpty(minDistanceProperty)) MinDistance = double.Parse(minDistanceProperty);
            if (!string.IsNullOrEmpty(maxDistanceProperty)) MaxDistance = double.Parse(maxDistanceProperty);
        }
        
        public override bool IsValid()
        {
            return base.IsValid() && Properties.Length > 0;
        }

        private string GetMinDistanceProperty()
        {
            var property = _spatialiserEffectClassObject.GetProperty(EZFMODMetadataGlossary.Property.MIN_DISTANCE);
            if (property == null) return string.Empty;
            return property.Values == null ? string.Empty : property.Values[0];
        }

        private string GetMaxDistanceProperty()
        {
            var property = _spatialiserEffectClassObject.GetProperty(EZFMODMetadataGlossary.Property.MAX_DISTANCE);
            if (property == null) return string.Empty;
            return property.Values == null ? string.Empty : property.Values[0];
        }
    }
}