#if UNITY_EDITOR
using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;
#pragma warning disable CS0618

namespace OCSFXEditor.FMOD
{
    public static class EditorMenuItems
    {
        [MenuItem("OCSFX/Setup FMOD Listener(s)")]
        public static void SetFmodAudioListeners()
        {
            var audioListeners = Object.FindObjectsOfType<AudioListener>();

            foreach (var listener in audioListeners)
            {
                var owner = listener.gameObject;
                if (!owner.TryGetComponent<StudioListener>(out var fmodListener))
                    fmodListener = owner.AddComponent<StudioListener>();
                else
                {
                    Debug.Log($"{owner.name} already has a {nameof(StudioListener)}");
                    continue;
                }

                if (!fmodListener)
                {
                    Debug.LogWarning($"Failed to ensure a {nameof(StudioListener)} on {owner.name}");
                    return;
                }
                
                Object.DestroyImmediate(listener);
            }

            if (audioListeners.Length < 1)
            {
                var cameras = Object.FindObjectsOfType<Camera>();
                
                foreach (var camera in cameras)
                {
                    var owner = camera.gameObject;
                    if (!owner.TryGetComponent<StudioListener>(out var fmodListener))
                        fmodListener = owner.AddComponent<StudioListener>();
                    else
                    {
                        Debug.Log($"{owner.name} already has a {nameof(StudioListener)}");
                        continue;
                    }

                    if (!fmodListener)
                    {
                        Debug.LogWarning($"Failed to ensure a {nameof(StudioListener)} on {owner.name}");
                        return;
                    }
                }
            }
            
            DestroyDuplicateListeners();
        }

        private static void DestroyDuplicateListeners()
        {
            var fmodListeners = Object.FindObjectsOfType<StudioListener>();

            if (fmodListeners.Length < 1) return;

            var owners = new HashSet<GameObject>();

            foreach (var listener in fmodListeners)
            {
                owners.Add(listener.gameObject);
            }

            foreach (var obj in owners)
            {
                var listeners = obj.GetComponents<StudioListener>();

                if (listeners.Length <= 1) continue;
                
                for (int i = 1; i < listeners.Length; i++)
                {
                    Object.DestroyImmediate(listeners[i]);
                }
            }
        }
    }
}
#endif
