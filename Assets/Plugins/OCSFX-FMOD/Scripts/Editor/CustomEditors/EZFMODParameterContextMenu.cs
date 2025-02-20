using OCSFX.EZFMOD.Types;
using UnityEditor;
using UnityEngine;
using static OCSFX.EZFMODEditor.EZFMODEditorStatics;

namespace OCSFX.EZFMODEditor.CustomEditors
{
        public static class EZFMODParameterContextMenu
        {
            private const string _CREATE_MENU_ITEM =
                "Assets/" + MENU_ITEM_ROOT + "/Create " + nameof(EZFMODParameterValue);

            private const string _DELETE_MENU_ITEM =
                "Assets/" + MENU_ITEM_ROOT + "/Delete " + nameof(EZFMODParameterValue);
            
            private const string _RENAME_MENU_ITEM =
                "Assets/" + MENU_ITEM_ROOT + "/Rename " + nameof(EZFMODParameterValue);

            private const string _OBJECT_TYPE_NAME = "EZFMOD Parameter Value Asset";
                
            
            [MenuItem(_CREATE_MENU_ITEM, false, 100)]
            public static void AddNewParamValueSubobject()
            {
                var ezfmodParameter = Selection.activeObject as EZFMODParameter;
                if (!ezfmodParameter)
                {
                    Debug.LogWarning($"Selected object is not {nameof(EZFMODParameter)}.");
                    return;
                }

                var window = ScriptableObject.CreateInstance<EZFMODParameterValueWindow>();
                window.titleContent = new GUIContent($"Create new {_OBJECT_TYPE_NAME} for {ezfmodParameter.Name}");
                window.inputName = ezfmodParameter.Name + "_UserValue_01";
                window.onNameEntered = newName =>
                {
                    if (string.IsNullOrEmpty(newName))
                    {
                        Debug.LogWarning($"{_OBJECT_TYPE_NAME} creation canceled or invalid name.");
                        return;
                    }

                    var newSubobject = ScriptableObject.CreateInstance<EZFMODParameterValue>();
                    newSubobject.Init(ezfmodParameter, ezfmodParameter.Default);
                    newSubobject.name = newName;
                    AssetDatabase.MakeEditable(AssetDatabase.GetAssetPath(ezfmodParameter));
                    EditorUtility.SetDirty(newSubobject);
                    AssetDatabase.AddObjectToAsset(newSubobject, ezfmodParameter);
                    EditorUtility.SetDirty(ezfmodParameter);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ezfmodParameter),
                        ImportAssetOptions.ForceUpdate);
                    AssetDatabase.Refresh();
                };
                window.ShowUtility();
            }

            [MenuItem(_CREATE_MENU_ITEM, true)]
            private static bool ValidateAddNewSubobject()
            {
                return IsValidContinuousParameter(Selection.activeObject);
            }

            [MenuItem(_DELETE_MENU_ITEM, false, 101)]
            public static void DeleteParamValueSubobject()
            {
                var ezfmodParameterValue = Selection.activeObject as EZFMODParameterValue;
                if (!ezfmodParameterValue)
                {
                    Debug.LogWarning($"Selected object is not {nameof(EZFMODParameterValue)}.");
                    return;
                }

                bool confirmDelete = EditorUtility.DisplayDialog(
                    "Confirm Delete",
                    "Are you sure you want to delete this subobject? This action cannot be undone.",
                    "Delete",
                    "Cancel"
                );

                if (!confirmDelete)
                {
                    return;
                }

                var assetPath = AssetDatabase.GetAssetPath(ezfmodParameterValue);
                Object.DestroyImmediate(ezfmodParameterValue, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();
            }

            [MenuItem(_DELETE_MENU_ITEM, true)]
            private static bool ValidateDeleteSubobject()
            {
                return IsValidContinuousParameterValue(Selection.activeObject);
            }

            [MenuItem(_RENAME_MENU_ITEM, false, 102)]
            public static void RenameParamValueSubobject()
            {
                var ezfmodParameterValue = Selection.activeObject as EZFMODParameterValue;
                if (!ezfmodParameterValue)
                {
                    Debug.LogWarning($"Selected object is not {nameof(EZFMODParameterValue)}.");
                    return;
                }

                var window = ScriptableObject.CreateInstance<EZFMODParameterValueWindow>();
                window.titleContent = new GUIContent($"Rename {_OBJECT_TYPE_NAME} for {ezfmodParameterValue.Parameter.Name}");
                window.inputName = ezfmodParameterValue.name;
                window.onNameEntered = newName =>
                {
                    if (string.IsNullOrEmpty(newName))
                    {
                        Debug.LogWarning("Rename canceled or invalid name.");
                        return;
                    }

                    ezfmodParameterValue.name = newName;
                    AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ezfmodParameterValue),
                        ImportAssetOptions.ForceUpdate);
                    AssetDatabase.Refresh();
                };
                window.ShowUtility();
            }

            [MenuItem(_RENAME_MENU_ITEM, true)]
            private static bool ValidateRenameSubobject()
            {
                return IsValidContinuousParameterValue(Selection.activeObject);
            }

            private static bool IsValidContinuousParameter(Object selectedObject)
            {
                return selectedObject is EZFMODParameter ezfmodParameter 
                       && ezfmodParameter.Type == ParameterType.Continuous;
            }
            
            private static bool IsValidContinuousParameterValue(Object selectedObject)
            {
                return selectedObject is EZFMODParameterValue ezfmodParameterValue 
                       && ezfmodParameterValue.Parameter.Type == ParameterType.Continuous;
            }
        }
    
}