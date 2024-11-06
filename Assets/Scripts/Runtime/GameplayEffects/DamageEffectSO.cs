using OCSFX.FMOD;
using OCSFX.Utility.Debug;
using GameOff2024.GameplayEntities;
using UnityEngine;

namespace GameOff2024.GameplayEffects
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_NAME_BASE + _GAMEPLAY_EFFECTS_PATH + nameof(DamageEffectSO))]
    public class DamageEffectSO : GameplayEffectSO
    {
        public override bool ApplyEffect(GameplayEntityBase target)
        {
            if (target == null)
            {
                OCSFXLogger.LogWarning($"{nameof(ApplyEffect)} {nameof(GameplayEntityBase)} target is null.", this);
                return false;
            }
        
            if (VfxOnApply) Instantiate(VfxOnApply, target.Transform.position, Quaternion.identity);
            if (!SfxOnApply.IsNull) SfxOnApply.Play(target.Transform.gameObject);

            return target.TakeDamage(Amount);
        }
    }
}
