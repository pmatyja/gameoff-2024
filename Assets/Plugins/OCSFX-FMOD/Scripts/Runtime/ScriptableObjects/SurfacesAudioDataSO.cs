using System.Collections.Generic;
using System.Linq;
using OCSFX.FMOD.Types;
using UnityEngine;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Surfaces", fileName = nameof(SnapshotsAudioDataSO))]
    public class SurfacesAudioDataSO : AudioDataSO
    {
        [SerializeField] private AudioSurface _defaultSurface = AudioSurface.Stone;
        [SerializeField] private List<OCSFX.Generics.KeyValuePair<AudioSurface, PhysicsMaterial[]>> _surfaces = new();

        private readonly Dictionary<AudioSurface, PhysicsMaterial[]> _dictionary = new();

        public AudioSurface GetSurfaceType(PhysicsMaterial physMat)
        {
            var returnSurfaceType = _defaultSurface;
            
            foreach (var surface in _dictionary)
            {
                foreach (var physicMaterial in surface.Value)
                {
                    if (physMat.name.Contains(physicMaterial.name))
                    {
                        returnSurfaceType = surface.Key;
                    }
                }
            }

            return returnSurfaceType;
        }

        private void RefreshDictionary()
        {
            // Remove duplicates
            var keys = new HashSet<AudioSurface>();
            _surfaces = _surfaces
                .Where(surface => keys.Add(surface.Key))
                .ToList();

            // Refresh the internal Dictionary
            _dictionary.Clear();
            
            foreach (var entry in _surfaces
                         .Where(entry => entry?.Value.Length > 0))
            {
                _dictionary.TryAdd(entry.Key, entry.Value);
            }
        }
    
        private void OnValidate()
        {
            RefreshDictionary();
        }
    }
}
