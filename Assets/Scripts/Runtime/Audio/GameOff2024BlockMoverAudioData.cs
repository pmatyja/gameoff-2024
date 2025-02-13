using FMODUnity;
using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Types;
using UnityEngine;

namespace Runtime.Audio
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_ROOT + nameof(GameOff2024BlockMoverAudioData))]
    public class GameOff2024BlockMoverAudioData : ScriptableObject
    {
        [field: SerializeField] public EZFMODEvent MoveBeginEvent { get; private set; }
        [field: SerializeField] public EZFMODEvent MoveEndEvent { get; private set; }
        
        public void PlayBlockMoveBegin(GameObject blockObject)
        {
            if (!MoveBeginEvent) return;
            
            MoveBeginEvent.Play(blockObject);
        }
        
        public void StopBlockMoveBegin(GameObject blockObject)
        {
            if (!MoveBeginEvent) return;

            MoveBeginEvent.Stop(blockObject);
        }
        
        public void PlayBlockMoveEnd(GameObject blockObject)
        {
            if (!MoveEndEvent) return;
            
            MoveEndEvent.Play(blockObject);
        }
        
        public void StopBlockMoveEnd(GameObject blockObject)
        {
            if (!MoveEndEvent) return;

            MoveEndEvent.Stop(blockObject);
        }
    }
}