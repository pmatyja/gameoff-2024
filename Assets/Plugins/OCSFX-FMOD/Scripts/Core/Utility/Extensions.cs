using System.Collections.Generic;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.Utility
{
    public static class Extensions
    {
        public static bool Contains(this LayerMask layerMask, Component other)
        {
            return ((1 << other.gameObject.layer) & layerMask) != 0;
        }
        
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return ((1 << layer) & layerMask) != 0;
        }
        
        public static bool ContainsAny(this LayerMask layerMask, LayerMask otherMask)
        {
            return (layerMask & otherMask) != 0;
        }
        
        public static bool ContainsAll(this LayerMask layerMask, LayerMask otherMask)
        {
            return (layerMask & otherMask) == otherMask;
        }

#region List
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
        
        public static bool HasSameContentAs<T>(this List<T> list1, List<T> list2)
        {
            // Handle nulls. Treat null and empty the same
            if (list1 == null) return list2 == null || list2.Count == 0;
            if (list2 == null) return list1.Count == 0;
            
            if (list1.Count != list2.Count) return false;
            
            for (var i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i])) return false;
            }

            return true;
        }
        
        public static bool ContainsExactRange<T>(this List<T> source, List<T> range)
        {
            if (source.Count < range.Count) return false;

            var lastPossibleIndex = source.Count - range.Count;
            var match = true;

            for (var i = 0; i <= lastPossibleIndex; i++)
            {
                for (var j = range.Count - 1; j >= 0; j--)
                {
                    if (source[i + j].Equals(range[j])) continue;
                    
                    // If the current index doesn't match, then the range doesn't match
                    match = false;
                    break;
                }
            }

            return match;
        }

        public static bool ContainsExactRange<T>(this List<T> source, List<T> range, out int matchStartIndex)
        {
            matchStartIndex = -1;
            
            if (source.Count < range.Count) return false;

            var lastPossibleIndex = source.Count - range.Count;
            var match = true;

            for (var i = 0; i <= lastPossibleIndex; i++)
            {
                for (var j = range.Count - 1; j >= 0; j--)
                {
                    if (source[i + j].Equals(range[j])) continue;
                    
                    // If the current index doesn't match, then the range doesn't match
                    match = false;
                    break;
                }

                if (!match) continue;
                
                matchStartIndex = i;
                return true;
            }

            return false;
        }
        
#endregion List
        
#region Array
        public static T GetLooped<T>(this T[] array, int index)
        {
            return index < 0 ? array[array.Length] : array[index % array.Length];
        }

        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        
        public static void AddUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
        
        public static bool HasSameContentAs<T>(this T[] array1, T[] array2)
        {
            // Handle nulls. Treat null and empty the same
            if (array1 == null) return array2 == null || array2.Length == 0;
            if (array2 == null) return array1.Length == 0;
            
            if (array1.Length != array2.Length) return false;
            
            for (var i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i])) return false;
            }

            return true;
        }
        
        public static bool ContainsExactRange<T>(this T[] source, T[] range)
        {
            if (source.Length < range.Length) return false;

            var lastPossibleIndex = source.Length - range.Length;
            var match = true;

            for (var i = 0; i <= lastPossibleIndex; i++)
            {
                for (var j = range.Length - 1; j >= 0; j--)
                {
                    if (source[i + j].Equals(range[j])) continue;
                    
                    // If the current index doesn't match, then the range doesn't match
                    match = false;
                    break;
                }
            }

            return match;
        }
        
        public static bool ContainsExactRange<T>(this T[] source, T[] range, out int matchStartIndex)
        {
            matchStartIndex = -1;
            
            if (source.Length < range.Length) return false;

            var lastPossibleIndex = source.Length - range.Length;
            var match = true;

            for (var i = 0; i <= lastPossibleIndex; i++)
            {
                for (var j = range.Length - 1; j >= 0; j--)
                {
                    if (source[i + j].Equals(range[j])) continue;
                    
                    // If the current index doesn't match, then the range doesn't match
                    match = false;
                    break;
                }

                if (!match) continue;
                
                matchStartIndex = i;
                return true;
            }

            return false;
        }
        
#endregion Array

#region Float
        public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            // Ensure the value is within the original range
            value = Mathf.Clamp(value, Mathf.Min(fromMin, fromMax), Mathf.Max(fromMin, fromMax));
            
            // Map the value to the new range
            float mappedValue = (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
            
            // Ensure the mapped value is within the target range
            return Mathf.Clamp(mappedValue, Mathf.Min(toMin, toMax), Mathf.Max(toMin, toMax));
        }
        
        public static float Map01(this float value, float fromMin, float fromMax)
        {
            return Map(value, fromMin, fromMax, 0, 1);
        }
        
#endregion Float
        
#region GameObject
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent<T>(out var component))
                component = gameObject.AddComponent<T>();

            return component;
        }
        
        public static bool TryResolveComponent<T>(this GameObject gameObject, ref T component) where T : Component
        {
            if (component)
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
#endregion GameObject

    }
}
