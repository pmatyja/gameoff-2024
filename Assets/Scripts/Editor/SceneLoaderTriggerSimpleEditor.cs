using Runtime.World;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SceneLoaderTriggerSimple))]
    public class SceneLoaderTriggerSimpleEditor : UnityEditor.Editor
    {
        private SerializedProperty _showDebug;
        private bool _showDebugValue;
        
        public override void OnInspectorGUI()
        {
            var sceneLoaderTrigger = (SceneLoaderTriggerSimple)target;
            
            _showDebug = serializedObject.FindProperty(nameof(_showDebug));

            // Draw the script field
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(sceneLoaderTrigger), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            // Get the list of scenes in the build settings
            var scenes = EditorBuildSettings.scenes;
            var sceneNames = new string[scenes.Length];
            for (var i = 0; i < scenes.Length; i++)
            {
                sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
            }

            // Draw the dropdown list
            var selectedIndex = Mathf.Max(0, System.Array.IndexOf(sceneNames, sceneLoaderTrigger.SceneName));
            selectedIndex = EditorGUILayout.Popup("Scene", selectedIndex, sceneNames);
            sceneLoaderTrigger.SceneName = sceneNames[selectedIndex];
            
            // Draw the player tag field
            sceneLoaderTrigger.PlayerTag = EditorGUILayout.TagField("Player Tag", sceneLoaderTrigger.PlayerTag);

            // Draw the debug field
            _showDebugValue = _showDebug.boolValue = EditorGUILayout.Toggle("Show Debug", _showDebugValue);
            
            // Save changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(sceneLoaderTrigger);
            }
        }
    }
}