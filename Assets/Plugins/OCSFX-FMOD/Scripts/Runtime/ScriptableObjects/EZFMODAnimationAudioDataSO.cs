using System.Collections.Generic;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Animation", fileName = nameof(EZFMODAnimationAudioDataSO))]
    public class EZFMODAnimationAudioDataSO : EZFMODAudioDataSO
    {
        [Header("Animation Events")]
        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODEvent>> _animEvents = new()
        {
            new SerializedKeyValuePair<string, EZFMODEvent>("Footstep", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("Jump", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("Land", null)
        };
        
        public bool TryGetAnimEvent(string animEventName, out EZFMODEvent soundEvent)
        {
            soundEvent = _animEvents.Find(animEvent 
                => animEvent.Key == animEventName).Value;
            
            return soundEvent;
        }
    }
}