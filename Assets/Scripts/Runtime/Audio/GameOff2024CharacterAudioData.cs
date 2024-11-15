using FMODUnity;
using UnityEngine;

namespace Runtime.Audio
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_ROOT + nameof(GameOff2024CharacterAudioData))]
    public class GameOff2024CharacterAudioData : ScriptableObject
    {
        [field: SerializeField] public EventReference Footstep { get; private set; }
        [field: SerializeField] public EventReference Jump { get; private set; }
        [field: SerializeField] public EventReference Land { get; private set; }
        [field: SerializeField] public EventReference FoleyOneShot { get; private set; }
    }
}