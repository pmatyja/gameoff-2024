using System.Collections.Generic;
using OCSFX.Utility.Debug;
using UnityEngine;

namespace OCSFX.Utility
{
    public static class Extensions
    {
        public static bool Contains(this LayerMask layerMask, Component other)
        {
            return ((1 << other.gameObject.layer) & layerMask) != 0;
        }

        // Lists
        public static void Flush<T>(this List<T> list) where T : Object
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null) list.Remove(list[i]);
            }
        }

        public static T GetLooped<T>(this List<T> list, int index)
        {
            return index < 0 ? list[list.Count] : list[index % list.Count];
        }
        
        public static T GetRandom<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
        
        // Arrays
        public static T GetLooped<T>(this T[] array, int index)
        {
            return index < 0 ? array[array.Length] : array[index % array.Length];
        }

        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            // Ensure the value is within the original range
            value = Mathf.Clamp(value, fromMin, fromMax);

            // Map the value to the new range
            float mappedValue = (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;

            // Ensure the mapped value is within the target range
            return Mathf.Clamp(mappedValue, toMin, toMax);
        }
        
        public static float Map01(this float value, float fromMin, float fromMax)
        {
            return (value - fromMin) / (fromMax - fromMin);
        }
        
        public static T GetOrAdd<T>(this GameObject attachGameObject) where T : Component
        {
            if (!attachGameObject.TryGetComponent<T>(out var component))
                component = attachGameObject.AddComponent<T>();

            return component;
        }
        
        public static bool TryResolveComponent<T>(this GameObject gameObject, ref T component) where T : Component
        {
            if (component != null)
            {
                return true;
            }

            if (gameObject.TryGetComponent(out component))
            {
                return true;
            }

            OCSFXLogger.LogError($"No {typeof(T).Name} found on " + gameObject.name, gameObject);
            return false;
        }
    }
}
