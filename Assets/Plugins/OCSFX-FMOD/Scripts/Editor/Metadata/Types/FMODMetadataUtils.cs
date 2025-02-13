using System.Linq;
using OCSFX.EZFMODEditor.Metadata.Deserialization;
using UnityEngine;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public static class FMODMetadataUtils
    {
        public const double EZFMOD_EPSILON = 0.0000001;
        
        public static bool GetIsOneShot(FMODMetadata metadata)
        {
            /* TODO: Improve how we determine non-oneshot.
             Also needs to account for transition markers and regions which go back to some earlier destination marker or region.
            */
            foreach (var metadataObject in metadata.Objects)
                if (IsLoopClass(metadataObject) || ContainsLoopProperty(metadataObject))
                    return false;

            return true;
        }

        private static bool IsLoopClass(FMODMetadataObject metadataObject)
        {
            return metadataObject.Class
                is EZFMODMetadataGlossary.Class.SUSTAIN_POINT
                or EZFMODMetadataGlossary.Class.LOOP_REGION
                or EZFMODMetadataGlossary.Class.MAGNET_REGION;
        }

        private static bool ContainsLoopProperty(FMODMetadataObject metadataObject)
        {
            return metadataObject.Properties.Any(property => property.Name.Contains("loop"));
        }

        public static void TestParseMetadata(FMODMetadata metadata, string firstClassType, string firstClassName)
        {
            if (metadata.Objects.Count == 0)
            {
                Debug.LogError("No objects found in metadata.");
                return;
            }

            if (!metadata.Objects[0].Class.Equals(firstClassType))
            {
                Debug.Log($"Metadata is not of type {firstClassType}");
                return;
            }

            var firstObject = metadata.Objects[0];
            var firstObjectNamePropertyValues = firstObject.GetProperty(EZFMODMetadataGlossary.Attribute.NAME).Values;
            var firstObjectNameProperty =
                firstObjectNamePropertyValues.FirstOrDefault(value => value.Equals(firstClassName));
            if (firstObjectNameProperty == null) return;

            // this is the metadata we're filtering for...
            // print each object and its properties and relationships

            foreach (var metadataObject in metadata.Objects)
            {
                Debug.Log("===================================");

                Debug.Log($"Object: {metadataObject.Class} - {metadataObject.Id}");

                if (metadataObject.Class == EZFMODMetadataGlossary.Class.SPATIALISER_EFFECT)
                    Debug.Log($"Spatialiser Effect found with Id: {metadataObject.Id}");

                foreach (var property in metadataObject.Properties)
                {
                    Debug.Log($"Property: {property.Name} - {string.Join(", ", property.Values)}");
                    if (property.Name.Contains("loop"))
                        Debug.LogWarning($"Loop property found in {metadataObject.Class} object: {metadataObject.Id}");
                }

                foreach (var relationship in metadataObject.Relationships)
                    Debug.Log($"Relationship: {relationship.Name} - {string.Join(", ", relationship.Destinations)}");

                Debug.Log("-----------------------------------");
            }
        }
    }
}