using FMODUnity;
using GameOff2024.GameplayEntities;
using UnityEngine;

namespace GameOff2024.GameplayEffects
{
    public abstract class GameplayEffectSO : ScriptableObject
    {
        protected const string _GAMEPLAY_EFFECTS_PATH = "GameplayEffects/";
    
        [field: SerializeField] public int Amount { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public ParticleSystem VfxOnApply { get; private set; }
        [field: SerializeField] public EventReference SfxOnApply { get; private set; }

        public abstract bool ApplyEffect(GameplayEntityBase target);

        public virtual bool RemoveEffect(GameplayEntityBase target)
        {
            return false;
        }
    }
}
