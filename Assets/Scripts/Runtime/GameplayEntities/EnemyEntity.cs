using GameOff2024.Health;
using UnityEngine;

namespace GameOff2024.GameplayEntities
{
    public class EnemyEntity : GameplayEntityBase
    {
        [SerializeField] private CombatComponent _combatComponent;

        private void Awake()
        {
            if (!_combatComponent)
            {
                _combatComponent = GetComponentInChildren<CombatComponent>();
            }
        }
    
        public void BeginAttack()
        {
            _combatComponent.BeginAttack();
        }
    
        public void EndAttack()
        {
            _combatComponent.EndAttack();
        }
    }
}
