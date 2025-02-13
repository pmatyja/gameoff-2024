using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace OCSFX.EZFMODEditor.Metadata.Deserialization
{
    public static class EZFMODMetadataValidation
    {
        private static readonly List<string> _printedMessages = new List<string>();
        
        public static void ValidateFMODMetadata(FMODMetadata metadata)
        {
            _printedMessages.Clear();
            
            NotNull(metadata);
            HasObjects(metadata);
            IsValidObject(metadata.Objects[0], metadata);

            foreach (var metadataObject in metadata.Objects)
            {
                IsValidObject(metadataObject, metadata);
            }
        }

        private static void NotNull<T>(T obj)
        {
            Assert.IsTrue(obj != null, $"{nameof(T)} is null.");
        }
        
        private static void HasObjects(FMODMetadata metadata)
        {
            Assert.IsTrue(metadata.Objects is { Count: > 0 }, $"{nameof(FMODMetadata)} does not contain any objects.");
        }
        
        private static void IsValidObject(FMODMetadataObject metadataObject, FMODMetadata parentMetadata)
        {
            NotNull(metadataObject);
            HasValidClassName(metadataObject, parentMetadata);

            // if (metadataObject.Properties is { Count: 0 })
            // {
            //     PrintMessageWithClassNameAndId(LogType.Warning,$"{nameof(FMODMetadataObject)} does not contain any properties.", metadataObject, parentMetadata);
            // }
            //
            // if (metadataObject.Relationships is { Count: 0 })
            // {
            //     PrintMessageWithClassNameAndId(LogType.Warning, $"{nameof(FMODMetadataObject)} does not contain any relationships.", metadataObject, parentMetadata);
            // }
        }
        
        private static void PrintMessageWithClassNameAndId(LogType logType, string message, FMODMetadataObject metadataObject, FMODMetadata parentMetadata)
        {
            var fullMessage = $"(Class: {metadataObject.Class} | Id: {metadataObject.Id}) | {message} | [File: {parentMetadata.FileName}]";
            
            if (_printedMessages.Contains(fullMessage)) return;
            
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(fullMessage);
                    break;
                case LogType.Error:
                    Debug.LogError(fullMessage);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(fullMessage);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(fullMessage);
                    break;
                case LogType.Exception:
                    Debug.LogException(new Exception(fullMessage));
                    break;
            }
            
            _printedMessages.Add(fullMessage);
        }

        private static void HasValidClassName(FMODMetadataObject metadataObject, FMODMetadata parentMetadata)
        {
            Assert.IsTrue(
                !string.IsNullOrEmpty(metadataObject.Class), $"{nameof(FMODMetadataObject)} is missing a Class Attribute."
            );
        }
        
        // private static void HasValidName(FMODMetadataProperty metadataProperty)
        // {
        //     Assert.IsTrue(
        //         !string.IsNullOrEmpty(metadataProperty.Name), $"{nameof(FMODMetadataProperty)} is missing a Name Attribute."
        //     );
        // }
        //
        // private static void HasValidValues(FMODMetadataProperty metadataProperty)
        // {
        //     Assert.IsTrue(
        //         metadataProperty.Values is { Count: > 0 }, $"{nameof(FMODMetadataProperty)} is missing a Value Element."
        //     );
        // }
        //
        // private static void HasValidDestination(FMODMetadataRelationship metadataRelationship)
        // {
        //     Assert.IsTrue(
        //         metadataRelationship.Destinations is { Count: > 0 }, $"{nameof(FMODMetadataRelationship)} is missing a Destination Element."
        //     );
        // }
        //
        // private static void HasValidName(FMODMetadataRelationship metadataRelationship)
        // {
        //     Assert.IsTrue(
        //         !string.IsNullOrEmpty(metadataRelationship.Name), $"{nameof(FMODMetadataRelationship)} is missing a first Name Attribute."
        //     );
        // }
        
        private static void HasAllValidRelationships(FMODMetadataObject metadataObject)
        {
            var metadataRelationships = metadataObject.Relationships;
            
            Assert.IsTrue(metadataRelationships is { Count: > 0 }, $"{nameof(FMODMetadataObject)} (Class: {metadataObject.Class} | Id: {metadataObject.Id}) does not contain any relationships.");
            foreach (var relationship in metadataRelationships)
            {
                IsValidRelationship(relationship);
            }
        }
        
        private static void IsValidRelationship(FMODMetadataRelationship metadataRelationship)
        {
            Assert.IsTrue(
                metadataRelationship != null 
                && !string.IsNullOrEmpty(metadataRelationship.Name) 
                && metadataRelationship.Destinations is { Count: > 0 }, $"{nameof(FMODMetadataRelationship)} is missing a first Name Attribute or Destination Element."
            );
        }
        
        private static void HasAllValidProperties(FMODMetadataObject metadataObject)
        {
            Assert.IsTrue(metadataObject.Properties is { Count: > 0 }, 
                $"{nameof(FMODMetadataObject)} (Class: {metadataObject.Class} | Id: {metadataObject.Id}) does not contain any properties.");
            foreach (var property in metadataObject.Properties)
            {
                IsValidProperty(property);
            }
        }
        
        private static void IsValidProperty(FMODMetadataProperty metadataProperty)
        {
            Assert.IsTrue(
                metadataProperty != null 
                && !string.IsNullOrEmpty(metadataProperty.Name) 
                && metadataProperty.Values is { Count: > 0 }, $"{nameof(FMODMetadataProperty)} is missing a first Name Attribute or Value Element."
            );
        }
    }
}