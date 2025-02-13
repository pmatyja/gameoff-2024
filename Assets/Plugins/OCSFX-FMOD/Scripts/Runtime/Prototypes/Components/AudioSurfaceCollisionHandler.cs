using System.Collections.Generic;
using System.Linq;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility;
using UnityEngine;

namespace OCSFX.EZFMOD.Prototypes.Components
{
    public class AudioSurfaceCollisionHandler : MonoBehaviour
    {
        private readonly List<AudioSurfaceCollision> _audioSurfaceCollisions = new List<AudioSurfaceCollision>();
        
        [SerializeField, ReadOnly] private EZFMODParameterValue _currentSurface;
        
        
        public void OnSurfaceEnter(AudioSurfaceCollision audioSurfaceCollision)
        {
            _audioSurfaceCollisions.AddUnique(audioSurfaceCollision);
            
            SetCurrentSurface(GetHighestPrioritySurface());
        }

        public void OnSurfaceExit(AudioSurfaceCollision audioSurfaceCollision)
        {
            if (_audioSurfaceCollisions.Contains(audioSurfaceCollision))
            {
                _audioSurfaceCollisions.Remove(audioSurfaceCollision);
            }
            
            SetCurrentSurface(GetHighestPrioritySurface());
        }
        
        private EZFMODParameterValue GetHighestPrioritySurface()
        {
            return _audioSurfaceCollisions
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault()?.Surface;
        }
        
        private void SetCurrentSurface(EZFMODParameterValue surface)
        {
            if (_currentSurface == surface) return;
            
            _currentSurface = surface;
            
            _currentSurface.Set(gameObject);
        }
    }
}