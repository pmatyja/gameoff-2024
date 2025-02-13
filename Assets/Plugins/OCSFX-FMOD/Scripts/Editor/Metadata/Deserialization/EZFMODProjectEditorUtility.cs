using System.IO;
using FMODUnity;
using UnityEngine;

namespace OCSFX.EZFMODEditor.Metadata.Deserialization
{
    internal static class EZFMODProjectEditorUtility
    {
        private const string _XML_FILE_EXTENSION = "*.xml";

        public static FMODProjectMetadata GetFMODProjectMetadata()
        {
            if (!TryGetFMODProjectDirectory(out var projectDirectory))
            {
                return FMODProjectMetadata.Empty;
            }

            var metaData = GetFMODProjectMetadataFiles(projectDirectory);

            if (metaData.IsValid())
            {
                return metaData;
            }
            
            Debug.LogError($"{nameof(FMODProjectMetadata)} is not valid. Please ensure the metadata files are set up correctly.");
            return FMODProjectMetadata.Empty;
        }
        
        private static string[] GetXmlFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, _XML_FILE_EXTENSION);
        }

        public static bool TryGetFMODProjectDirectory(out string projectDirectory)
        {
            projectDirectory = string.Empty;
            
            var fmodSettings = Settings.Instance;
            
            if (!fmodSettings)
            {
                Debug.LogError("FMOD Settings not found. Please ensure FMOD Settings are set up in the project.");
                return false;
            }
            
            if (string.IsNullOrEmpty(fmodSettings.SourceProjectPath))
            {
                Debug.LogError($"FMOD Project Path not found. Please ensure FMOD Project Path is set up in {nameof(Settings)} Instance.");
                return false;
            }

            var relativeProjectPath = fmodSettings.SourceProjectPath;
            
            if (string.IsNullOrWhiteSpace(relativeProjectPath))
            {
                Debug.LogError($"FMOD Project Path not found. Please ensure FMOD Project Path is set up in {nameof(Settings)} Instance.");
                return false;
            }
            
            var projectPath = Path.GetFullPath(relativeProjectPath);
            
            Debug.Log($"[{nameof(EZFMOD)}] Verifying FMOD Project at path: {projectPath}");
            
            projectDirectory = Path.GetDirectoryName(projectPath);
            
            if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
            {
                Debug.LogError($"FMOD Project Directory not found. Please ensure FMOD Project Directory is set up in {nameof(FMODUnity.Settings)} Instance." +
                               $"\nAttempted path: {projectDirectory}");
                return false;
            }

            return true;
        }

        private static FMODProjectMetadata GetFMODProjectMetadataFiles(string FMODProjectDirectory)
        {
            var projectMetadataDirectory = Path.Combine(FMODProjectDirectory, EZFMODMetadataGlossary.Class.METADATA);
            
            // Check only specific directories
            var parameterPresetDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.PARAMETER_PRESET);
            var parameterPresetFolderDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.PARAMETER_PRESET_FOLDER);
            var bankDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.BANK);
            var bankFolderDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.BANK_FOLDER);
            var eventDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.EVENT);
            var eventFolderDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.EVENT_FOLDER);
            var audioFileDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.AUDIO_FILE);
            var effectPresetDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.EFFECT_PRESET);
            var snapshotDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.SNAPSHOT);
            var snapshotGroupDirectory = Path.Combine(projectMetadataDirectory, EZFMODMetadataGlossary.Class.SNAPSHOT_GROUP);

            var metaData = new FMODProjectMetadata.Builder()
                .SetParameterPresetMetadataFiles(GetXmlFiles(parameterPresetDirectory))
                .SetParameterPresetFolderMetadataFiles(GetXmlFiles(parameterPresetFolderDirectory))
                .SetBankMetadataFiles(GetXmlFiles(bankDirectory))
                .SetBankFolderMetadataFiles(GetXmlFiles(bankFolderDirectory))
                .SetEventMetadataFiles(GetXmlFiles(eventDirectory))
                .SetEventFolderMetadataFiles(GetXmlFiles(eventFolderDirectory))
                .SetAudioFileMetadataFiles(GetXmlFiles(audioFileDirectory))
                .SetEffectPresetMetadataFiles(GetXmlFiles(effectPresetDirectory))
                .SetSnapshotMetadataFiles(GetXmlFiles(snapshotDirectory))
                .SetSnapshotGroupMetadataFiles(GetXmlFiles(snapshotGroupDirectory))
                .Build();
            
            if (!metaData.IsValid())
            {
                Debug.LogError("FMOD Project Metadata is not valid. Please ensure the metadata files are set up correctly.");
            }
            
            return metaData;
        }
    }
}