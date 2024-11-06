using System;
using GameOff2024.Health;
using UnityEngine;

namespace GameOff2024.GameplayEntities
{
    public class HeroEntity : GameplayEntityBase
    {
        public static event Action<HealthComponent> HealthValueChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            HealthValueChanged = null;
        }

        private void OnEnable()
        {
            _healthComponent.ValueSet += OnHealthValueSet;
            _healthComponent.Damaged += OnDamaged;
            _healthComponent.Healed += OnHealed;
        
            _healthComponent.Died += OnDied;
        }

        private void OnDisable()
        {
            _healthComponent.ValueSet -= OnHealthValueSet;
            _healthComponent.Damaged -= OnDamaged;
            _healthComponent.Healed -= OnHealed;
        
            _healthComponent.Died -= OnDied;
        }
    
        private void OnHealthValueSet(int newValue) => HealthValueChanged?.Invoke(_healthComponent);
        private void OnDamaged(int damageAmount) => HealthValueChanged?.Invoke(_healthComponent);
        private void OnHealed(int healAmount) => HealthValueChanged?.Invoke(_healthComponent);

        private void OnDied()
        {
        }
    
        private void Respawn()
        {
        }
    }
}
