using OCSFX.FMOD;
using OCSFX.Utility.Debug;
using GameOff2024.GameplayEntities;
using UnityEngine;

namespace GameOff2024.GameplayEffects
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_NAME_BASE + _GAMEPLAY_EFFECTS_PATH + nameof(HealEffectSO))]
    public class HealEffectSO : GameplayEffectSO
    {
        public override bool ApplyEffect(GameplayEntityBase target)
        {
            if (target == null)
            {
                OCSFXLogger.LogWarning($"{nameof(ApplyEffect)} {nameof(GameplayEntityBase)} target is null.", this);
                return false;
            }
        
            if (VfxOnApply) Instantiate(VfxOnApply, target.transform.position, Quaternion.identity);
            if (!SfxOnApply.IsNull) SfxOnApply.Play(target.transform.gameObject);
        
            return target.Heal(Amount);
        }
    }
}
