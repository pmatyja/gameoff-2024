using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Types;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using OCSFX.EZFMOD.Debug;
using UnityEditor;
#endif //UNITY_EDITOR

namespace OCSFX.EZFMOD
{
    public class EZFMODSettings : ScriptableObject
    {
        // [field: SerializeField] public string PluginFolderPath { get; private set; } = EZFMODEditorStatics.PLUGIN_FOLDER_PATH;
        // [field: Tooltip("Relative to PluginFolderPath")]
        // [field: SerializeField] public string GeneratedAssetPath { get; private set; } = EZFMODEditorStatics.ASSET_OUTPUT_FOLDER_NAME;
        
        [field: Space]
        [field: Tooltip("(Default: TRUE.)\n" +
                        "Automatically generates and reconciles EZFMOD assets any time a change in the FMOD project bank files is detected.\n" +
                        "\n" +
                        "Manual reconciliation can be done via the " + EZFMODRuntimeStatics.MENU_ITEM_ROOT + " menu.")]
        [field: SerializeField] public bool ReconcileOnBankImport { get; private set; } = true;
        
        [field: Tooltip("(Default: TRUE.)\n" +
                        "Automatically generates and reconciles EZFMOD assets once per Editor session at startup.\n" +
                        "\n" +
                        "Manual reconciliation can be done via the " + EZFMODRuntimeStatics.MENU_ITEM_ROOT + " menu.")]
        [field: SerializeField] public bool ReconcileAtStartup { get; private set; } = true;
        
        [field: Space]
        [field: SerializeField, ReadOnly] public EZFMODBank MasterBank { get; private set; }
        [field: SerializeField] public bool LoadMasterBankOnGameStart{ get; private set; } = true;

        [field: SerializeField] public float MasterBanksPostLoadBuffer { get; private set; }
        [field: SerializeField] public EZFMODBank[] StartupBanks { get; private set; }
        
        // [field: Space]
        // [field: SerializeField] public Color AmbientZoneGizmoFillColor = Color.cyan * new Color(1, 1, 1, 0.25f);
        // [field: SerializeField] public Color AmbientZoneGizmoOutlineColor = Color.blue * new Color(1, 1, 1, 0.5f);
        //
        // [field: Space]
        // [field: SerializeField] public Color ReverbZoneGizmoFillColor = Color.yellow * new Color(1, 1, 1, 0.25f);
        // [field: SerializeField] public Color ReverbZoneGizmoOutlineColor = Color.grey * new Color(1, 1, 1, 0.5f);
        
        /*===========================================================*/
    
        private static EZFMODSettings _instance;
        
        internal static void SetMasterBank(EZFMODBank bank)
        {
            var instance = Get();
            if (instance.MasterBank == bank) return;
            
            instance.MasterBank = bank;
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
#endif
        }

        public static EZFMODSettings Get()
        {
            if (!_instance)
            {
                _instance = GetOrCreate();
            }

            return _instance;
        }

        private static EZFMODSettings GetOrCreate()
        {
            var assetInstance = Resources.Load<EZFMODSettings>(nameof(EZFMODSettings));
            
#if UNITY_EDITOR
            if (assetInstance) return assetInstance;
            
            var assetPath = EZFMODRuntimeStatics.PLUGIN_FOLDER_PATH + "/Resources";
            
            EnsureDirectoryExists(assetPath);

            assetInstance = GetOrCreateScriptableObjectAsset<EZFMODSettings>(
                assetPath, nameof(EZFMODSettings), out var createdNew);

            if (createdNew)
            {
                OCSFXLogger.Log($"New instance of {nameof(EZFMODSettings)} created at {assetPath}");
            }
#endif //UNITY_EDITOR

            return assetInstance;
        }
        
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Init() => EditorApplication.delayCall += () => Get();

        private static void EnsureDirectoryExists(string directoryPath)
        {
            if (!directoryPath.EndsWith("/")) directoryPath += "/";

            var directoryName = Path.GetDirectoryName(directoryPath);

            if (Directory.Exists(directoryName)) return;

            if (directoryName == null) return;

            Directory.CreateDirectory(directoryName);
            AssetDatabase.Refresh();
        }

        private static T GetOrCreateScriptableObjectAsset<T>(string assetPath, string assetName, out bool createdNew) where T : ScriptableObject
        {
            if (string.IsNullOrWhiteSpace(assetPath) || string.IsNullOrWhiteSpace(assetName))
            {
                createdNew = false;
                return null;
            }

            createdNew = false;

            var fullPath = $"{assetPath}/{assetName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if (asset) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssetIfDirty(asset);

            createdNew = true;

            return asset;
        }
#endif //UNITY_EDITOR
    }
}