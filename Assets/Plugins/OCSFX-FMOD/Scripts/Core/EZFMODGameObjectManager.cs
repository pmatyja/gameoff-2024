using System.Collections.Generic;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD
{
    internal class EZFMODGameObjectManager : SingletonMonoBehaviour<EZFMODGameObjectManager>
    {
        private readonly HashSet<EZFMODGameObject> _targetObjectsThisFrame = new HashSet<EZFMODGameObject>();
        private readonly HashSet<EZFMODGameObject> _registeredGameObjects = new HashSet<EZFMODGameObject>();
        
        private const float _INSTANCE_CLEANUP_INTERVAL = 1.0f;
        
        public static void RegisterEZFMODGameObject(EZFMODGameObject ezfmodGameObject)
        {
            if (Instance._registeredGameObjects.Count == 0)
            {
                SetRepeatingCleanup(true);
            }
            
            Instance._registeredGameObjects.Add(ezfmodGameObject);
        }
        
        public static void UnregisterEZFMODGameObject(EZFMODGameObject ezfmodGameObject)
        {
            if (!_instance) return;
            
            _instance._registeredGameObjects.Remove(ezfmodGameObject);
            
            if (_instance._registeredGameObjects.Count == 0)
            {
                SetRepeatingCleanup(false);
            }
        }
        
        private static void SetRepeatingCleanup(bool shouldInvoke)
        {
            if (shouldInvoke == _instance?.IsInvoking(nameof(CleanUpDeadInstances))) return;
            
            if (shouldInvoke) _instance?.InvokeRepeating(nameof(CleanUpDeadInstances), _INSTANCE_CLEANUP_INTERVAL, _INSTANCE_CLEANUP_INTERVAL);
            else _instance?.CancelInvoke(nameof(CleanUpDeadInstances));
        }
        
        private void CleanUpDeadInstances()
        {
            _targetObjectsThisFrame.UnionWith(_registeredGameObjects);
            
            foreach (var ezfmodGameObject in _targetObjectsThisFrame)
            {
                if (!ezfmodGameObject) continue;
                ezfmodGameObject.CleanUpDeadInstances();
            }
            
            _targetObjectsThisFrame.Clear();
            // OCSFXLogger.Log($"[{nameof(EZFMODGameObjectManager)}] Cleaned up dead instances.");
        }

        private static void Shutdown()
        {
            OCSFXLogger.Log($"[{nameof(EZFMODGameObjectManager)}] Shutdown.");

            if (!_instance) return;

            SetRepeatingCleanup(false);
            _instance._registeredGameObjects.Clear();
            DestroyImmediate(_instance.gameObject);
        }
    }
}