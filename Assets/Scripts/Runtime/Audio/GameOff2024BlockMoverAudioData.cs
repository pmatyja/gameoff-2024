using FMODUnity;
using OCSFX.FMOD;
using UnityEngine;

namespace Runtime.Audio
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_ROOT + nameof(GameOff2024BlockMoverAudioData))]
    public class GameOff2024BlockMoverAudioData : ScriptableObject
    {
        [field: SerializeField] public EventReference MoveBegin { get; private set; }
        [field: SerializeField] public EventReference MoveLoop { get; private set; }
        [field: SerializeField] public EventReference MoveEnd { get; private set; }
        
        public void PlayBlockMoveBegin(GameObject blockObject)
        {
            if (MoveBegin.IsNull) return;

            MoveBegin.Play(blockObject);
        }
        
        public void StopBlockMoveBegin(GameObject blockObject)
        {
            if (MoveBegin.IsNull) return;

            MoveBegin.Stop(blockObject);
        }
        
        public void PlayBlockMoveLoop(GameObject blockObject)
        {
            if (MoveLoop.IsNull) return;

            MoveLoop.Play(blockObject);
        }
        
        public void StopBlockMoveLoop(GameObject blockObject)
        {
            if (MoveLoop.IsNull) return;

            MoveLoop.Stop(blockObject);
        }
        
        public void PlayBlockMoveEnd(GameObject blockObject)
        {
            if (MoveEnd.IsNull) return;

            MoveEnd.Play(blockObject);
        }
        
        public void StopBlockMoveEnd(GameObject blockObject)
        {
            if (MoveEnd.IsNull) return;

            MoveEnd.Stop(blockObject);
        }
    }
}