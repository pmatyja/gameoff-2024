using GameOff2024.Health;
using UnityEngine;

namespace GameOff2024.GameplayEntities
{
    [RequireComponent(typeof(HealthComponent))]
    public abstract class GameplayEntityBase : MonoBehaviour, IDamagable, IHealable
    {
        [SerializeField] protected HealthComponent _healthComponent;

        private void Awake()
        {
            if (_healthComponent == null)
            {
                _healthComponent = GetComponent<HealthComponent>();
            }
        }

        public HealthComponent GetHealthComponent() => _healthComponent;

        public bool Heal(int amount) => _healthComponent.Heal(amount);

        public bool TakeDamage(int amount) => _healthComponent.TakeDamage(amount);

        public bool IsDead() => _healthComponent.IsDead();
    
        public Transform Transform => transform;

        protected void Reset()
        {
            _healthComponent = GetComponent<HealthComponent>();
        }
    }
}
