using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EditorOnly
{
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invoke(Action action)
    {
        #if UNITY_EDITOR
            action.Invoke();
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string OpenFolderPanel(string title, string folder = null, string defaultName = null)
    {
        #if UNITY_EDITOR
            return EditorUtility.OpenFolderPanel(title, folder ?? string.Empty, defaultName ?? string.Empty);
        #else
            return string.Empty;
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetAssetPath(int instanceID)
    {
        #if UNITY_EDITOR
            return AssetDatabase.GetAssetPath(instanceID);
        #else
            return string.Empty;
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetAssetPath(UnityEngine.Object instance)
    {
        #if UNITY_EDITOR
            return AssetDatabase.GetAssetPath(instance);
        #else
            return string.Empty;
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> FindAssets<T>() where T : UnityEngine.Object
    {
        #if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            return guids.Select(x => AssetDatabase.GUIDToAssetPath(x));
        #else
            return Enumerable.Empty<string>();
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FindAsset<T>(string name) where T : UnityEngine.Object
    {
        return EditorOnly.FindAssets<T>().FirstOrDefault(x => x.EndsWith(name));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LoadAsset<T>(string path, out T result) where T : UnityEngine.Object
    {
        result = LoadAsset<T>(path);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T LoadOrCreateAsset<T>(string path) where T : UnityEngine.ScriptableObject
    {
        if (TryLoadAsset<T>(path, out T result))
        {
            return result;
        }

        #if UNITY_EDITOR
            var instance = ScriptableObject.CreateInstance(typeof(T)) as T;
            UnityEditor.AssetDatabase.CreateAsset(instance, path);
            return instance;
        #else
            return default(T);
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryLoadAsset<T>(string path, out T result) where T : UnityEngine.Object
    {
        result = LoadAsset<T>(path);
        return result != null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        #if UNITY_EDITOR
             return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        #else
            return default(T);
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LoadAsset<T>(string path, string searchPattern, IList<T> results, SearchOption searchOption = SearchOption.TopDirectoryOnly) where T : UnityEngine.Object
    {
        if (results == null)
        {
            return;
        }

        if (Directory.Exists(path) == false)
        {
            return;
        }

        results.Clear();

        var files = Directory.GetFiles(path, searchPattern, searchOption);

        foreach (var file in files)
        {
            if (TryLoadAsset<T>(file, out var asset))
            { 
                results.Add(asset);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NicifyName(string name)
    {
        #if UNITY_EDITOR
            return UnityEditor.ObjectNames.NicifyVariableName(name);
        #else
            return name;
        #endif
    }

    public static IEnumerable<string> GetAnimationNames(RuntimeAnimatorController runtimeAnimatorController)
    {
        #if UNITY_EDITOR
            if (runtimeAnimatorController)
            {
                var results = new List<string>();

                if (runtimeAnimatorController is UnityEditor.Animations.AnimatorController editorAnimator)
                {
                    foreach (var layer in editorAnimator.layers)
                    {
                        foreach (var animatorState in layer.stateMachine.states)
                        {
                            results.Add(animatorState.state.name);
                        }
                    }
                }

                return results;
            }
        #endif

        return System.Linq.Enumerable.Empty<string>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> GetAnimationNames(Animator animator)
    {
        if (animator)
        {
            return GetAnimationNames(animator?.runtimeAnimatorController);
        }

        return System.Linq.Enumerable.Empty<string>();
    }

    public static void SetDirty(UnityEngine.Object obj)
    {
        #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
        #endif
    }

    public static void SaveAssets()
    {
        #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
        #endif
    }

    public static IEnumerable<T> FindAssets<T>(string filter, params string[] searchFolders) where T : UnityEngine.Object
    {
        #if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets(filter, searchFolders);
            return guids.Select(x => UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(x)));
        #else
            return Enumerable.Empty<T>();
        #endif
    }

    public static Texture2D GetIcon(string key)
    {
        #if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            return UnityEditor.EditorGUIUtility.IconContent(key)?.image as Texture2D;
        #else
            return null;
        #endif
    }

    public static Texture2D GetIcon(Type type, string defaultIcon = null)
    {
        var field = type?.GetField("Icon", BindingFlags.Static | BindingFlags.Public);
        return EditorOnly.GetIcon(field?.GetValue(null)?.ToString() ?? defaultIcon);
    }

    public static Texture2D GetIcon(object generic, string defaultIcon = null)
    {
        if (generic is Type type)
        {
            return EditorOnly.GetIcon(type, defaultIcon);
        }

        return EditorOnly.GetIcon(generic?.GetType(), defaultIcon);
    }

    public static IEnumerable<string> GetAllScenes()
    {
        #if UNITY_EDITOR
            var scenes = new List<string>();

            foreach(var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                { 
                    scenes.Add(Path.GetFileNameWithoutExtension(scene.path));
                }
            }

            return scenes;
        #else
            return Enumerable.Empty<string>();
        #endif
    }
}
