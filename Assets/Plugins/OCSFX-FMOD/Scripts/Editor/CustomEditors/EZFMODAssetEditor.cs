using OCSFX.EZFMOD.Types;
using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor.CustomEditors
{
    [CustomEditor(typeof(EZFMODAsset), true)]
    public class EZFMODAssetEditor<T> : Editor where T : EZFMODAsset
    {
        protected T _asset;
        
        public override void OnInspectorGUI()
        {
            DrawScriptField();
            
            _asset = GetAsset();
            if (!_asset) return;
            
            DrawBaseFields();
            EditorGUILayout.Space();
        }
        
        protected T GetAsset()
        {
            return (T)serializedObject.targetObject;
        }
        
        private void DrawScriptField()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((EZFMODAsset)target), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawBaseFields()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Name", _asset.Name);
            EditorGUILayout.TextField("Path", _asset.StudioPath);
            EditorGUILayout.TextField("GUID", _asset.GUID.ToString());
            EditorGUI.EndDisabledGroup();
        }
        
        protected static bool DrawObjectArray<TArray>(bool expand, string label, TArray[] array, bool disabled = true, GUIStyle style = null) where TArray : Object
        {
            style ??= EditorStyles.foldout;
            
            var returnExpand = EditorGUILayout.BeginFoldoutHeaderGroup(expand, label, style);
            if (expand)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(disabled);
                for (var i = 0; i < array.Length; i++)
                {
                    EditorGUILayout.ObjectField(i.ToString(), array[i], typeof(TArray), false);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            return returnExpand;
        }
        
        protected static bool DrawStringArray(bool expand, string label, string[] array, bool disabled = true, GUIStyle style = null)
        {
            style ??= EditorStyles.foldout;
            
            var returnExpand = EditorGUILayout.BeginFoldoutHeaderGroup(expand, label, style);
            if (expand)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(disabled);
                for (var i = 0; i < array.Length; i++)
                {
                    EditorGUILayout.TextField(i.ToString(), array[i]);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            return returnExpand;
        }
        
        protected static bool DrawFloatArray(bool expand, string label, float[] array, bool disabled = true, GUIStyle style = null)
        {
            style ??= EditorStyles.foldout;
            
            var returnExpand = EditorGUILayout.BeginFoldoutHeaderGroup(expand, label, style);
            if (expand)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(disabled);
                for (var i = 0; i < array.Length; i++)
                {
                    EditorGUILayout.FloatField(i.ToString(), array[i]);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            return returnExpand;
        }
        
        protected static bool DrawIntArray(bool expand, string label, int[] array, bool disabled = true, GUIStyle style = null)
        {
            style ??= EditorStyles.foldout;
            
            var returnExpand = EditorGUILayout.BeginFoldoutHeaderGroup(expand, label, style);
            if (expand)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(disabled);
                for (var i = 0; i < array.Length; i++)
                {
                    EditorGUILayout.IntField(i.ToString(), array[i]);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            return returnExpand;
        }
    }
    
    [CustomEditor(typeof(EZFMODAsset), true)]
    public class EZFMODAssetEditor : EZFMODAssetEditor<EZFMODAsset>
    {
    }
}