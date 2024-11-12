using Runtime.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(BuildSceneNameAttribute))]
    public class BuildSceneNameAttributeDrawer : PropertyDrawer
    {
        private EditorBuildSettingsScene[] _scenes;
        private string[] _sceneNames;
        private int _selectedIndex;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "BuildSceneName attribute can only be used with string", MessageType.Error);
                return;
            }

            _scenes = EditorBuildSettings.scenes;
            _sceneNames = new string[_scenes.Length];
            for (var i = 0; i < _scenes.Length; i++)
            {
                _sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(_scenes[i].path);
            }

            _selectedIndex = Mathf.Max(0, System.Array.IndexOf(_sceneNames, property.stringValue));
            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, _sceneNames);
            property.stringValue = _sceneNames[_selectedIndex];
        }
    }
}