using System.Collections.Generic;
using FMODUnity;
using OCSFX.EZFMOD.Types;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Animation", fileName = nameof(EZFMODAnimationAudioDataSO))]
    public class EZFMODAnimationAudioDataSO : EZFMODAudioDataSO
    {
        [Header("Animation Events")]
        [SerializeField] private List<FMODEvent> _events = new List<FMODEvent>()
        {
            new FMODEvent("Footstep", new EventReference()),
            new FMODEvent("Jump", new EventReference()),
            new FMODEvent("Land", new EventReference())
        };
        
        public bool TryGetAnimEvent(string animEventName, out EventReference soundEvent)
        {
            soundEvent = GetAnimEvent(animEventName);
            return !soundEvent.IsNull;
        }

        public EventReference GetAnimEvent(string animEventName)
        {
            return _events.Find((fmodEventStruct) =>
                fmodEventStruct.Name == animEventName).EventRef;
        }
    }
}