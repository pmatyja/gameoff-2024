using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace OCSFX.FMOD.AudioData
{
    /// <summary>
    /// This script is a work in progress. Do not use.
    /// </summary>
    // [CreateAssetMenu(menuName = "OCSFX.FMOD/ObjectEventInstanceLimitManager")]
    public class FMODObjectInstanceLimitManager : AudioDataSO
    {
        private static FMODObjectInstanceLimitManager _instance;
        
        [SerializeField] private List<Generics.KeyValuePair<EventReference, int>> _perObjectLimits;

        private Dictionary<EventReference, int> EventLimitDictionary { get; } = new Dictionary<EventReference, int>();

        private void OnValidate()
        {
            if (_instance && _instance != this)
            {
                DestroyImmediate(this);
            }
            
            if (_perObjectLimits.Count < 1) return;
            
            EventLimitDictionary.Clear();
            
            foreach (var entry in _perObjectLimits)
            {
                entry.Value = Mathf.Clamp(entry.Value, 1, 128);

                EventLimitDictionary.TryAdd(entry.Key, entry.Value);
            }
        }

        public static int GetObjectEventInstanceLimit(EventReference eventRef)
        {
            if (_instance.EventLimitDictionary == null || _instance.EventLimitDictionary.Count < 1) return 0;

            _instance.EventLimitDictionary.TryGetValue(eventRef, out var limit);

            return limit;
        }
    }
}