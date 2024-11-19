using System;
using Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class EditorSceneLoadWindow : EditorWindow
    {
        private string _sceneName;
        private EditorBuildSettingsScene[] _scenes;
        private string[] _sceneNames;
        private int _selectedIndex;
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        
        [MenuItem(GameOff2024Statics.PROJECT_NAME + "/Load Scene")]
        private static void ShowWindow()
        {
            var window = GetWindow<EditorSceneLoadWindow>();
            window.titleContent = new GUIContent(GameOff2024Statics.PROJECT_NAME + " Scene Loader");
            window.Show();
        }

        private void CreateGUI()
        {
            // Set the dimensions of the window
            minSize = new Vector2(400, 100);
            maxSize = new Vector2(400, 100);
        }

        private void OnGUI()
        {
            DrawScenesDropdown();
            
            DrawHelpBox();

            DrawLoadSceneModeDropdown();
            
            DrawLoadSceneButton();
        }

        private static void DrawHelpBox()
        {
            // Draw a message to the user
            EditorGUILayout.HelpBox(
                "This will load the selected scene in the editor." +
                "\nIf the desired scene isn't in this list, it means that that scene has not been added to the Build Settings.",
                MessageType.Info);
        }

        private void DrawLoadSceneButton()
        {
            if (!GUILayout.Button("Load Scene")) return;
            
            // While in play mode
            if (Application.isPlaying)
            {
                SceneManager.LoadScene(_sceneName, _loadSceneMode);
                return;
            }

            // Outside of play mode.
            var cancelled = !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (cancelled) return;

            Save();
            EditorSceneManager.OpenScene(_scenes[_selectedIndex].path, (OpenSceneMode)_loadSceneMode);
        }

        private void DrawScenesDropdown()
        {
            // Get the list of scenes in the build settings
            _scenes = EditorBuildSettings.scenes;
            _sceneNames = new string[_scenes.Length];
            for (var i = 0; i < _scenes.Length; i++)
            {
                _sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(_scenes[i].path);
            }

            // Draw the dropdown list
            _selectedIndex = Mathf.Max(0, System.Array.IndexOf(_sceneNames, _sceneName));
            _selectedIndex = EditorGUILayout.Popup("Scene", _selectedIndex, _sceneNames);
            _sceneName = _sceneNames[_selectedIndex];
        }
        
        private void DrawLoadSceneModeDropdown()
        {
            _loadSceneMode = (LoadSceneMode) EditorGUILayout.EnumPopup("Load Scene Mode", _loadSceneMode);
        }

        private static void Save() => AssetDatabase.SaveAssets();
    }
}