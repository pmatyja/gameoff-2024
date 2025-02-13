using FMODUnity;
using OCSFX.EZFMOD.Types;
using UnityEngine;

namespace Runtime.Audio
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_ROOT + nameof(GameOff2024CharacterAudioData))]
    public class GameOff2024CharacterAudioData : ScriptableObject
    {
        [field: SerializeField] public EZFMODEvent Footstep { get; private set; }
        [field: SerializeField] public EZFMODEvent Jump { get; private set; }
        [field: SerializeField] public EZFMODEvent Land { get; private set; }
        [field: SerializeField] public EZFMODEvent FoleyOneShot { get; private set; }
    }
}