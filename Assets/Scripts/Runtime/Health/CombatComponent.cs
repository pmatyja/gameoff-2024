using System.Collections;
using System.Collections.Generic;
using OCSFX.Utility;
using OCSFX.Utility.Debug;
using GameOff2024.GameplayEffects;
using GameOff2024.GameplayEntities;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using OCSFX.Attributes;
#endif

namespace GameOff2024.Health
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CombatComponent : MonoBehaviour
    {
        [SerializeField] private LayerMask _attackLayers;
        [SerializeField] private CircleCollider2D _attackCollider;
        [SerializeField] private DamageEffectSO _damageEffectType;
    
        private readonly HashSet<GameplayEntityBase> _hitEntities = new HashSet<GameplayEntityBase>();
    
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

        private void Awake()
        {
            if (!_attackCollider)
            {
                _attackCollider = GetComponent<CircleCollider2D>();
            }

            _attackCollider.isTrigger = true;
            _attackCollider.enabled = false;
        }

#if UNITY_EDITOR
    [SerializeField, ReadOnly] private List<GameplayEntityBase> _debugHitEntities = new List<GameplayEntityBase>();
    private void UpdateDebugHitEntities()
    {
        _debugHitEntities = _hitEntities.ToList();
    }

    private void Update()
    {
        UpdateDebugHitEntities();
    }
#endif
        
        [field: SerializeField] public GameplayEntityBase Owner { get; private set; }

        private bool _isAttacking;
    
        private Coroutine _attackWindowCoroutine;
    
        public void BeginAttack()
        {
            if (_isAttacking) return;
        
            _attackCollider.isTrigger = true;
            _attackCollider.enabled = true;
            _attackCollider.radius = _damageEffectType.Radius;
        
            _isAttacking = true;
        
            HandleAttackWindow(_damageEffectType.Duration);
        }
    
        public void EndAttack()
        {
            if (!_isAttacking) return;
        
            _isAttacking = false;
            _hitEntities.Clear();
            _attackCollider.enabled = false;
        
            if (_attackWindowCoroutine != null)
            {
                StopCoroutine(_attackWindowCoroutine);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == gameObject) return;
        
            if (!_isAttacking) return;
        
            OCSFXLogger.Log($"{name} overlapped with {other.name}", this, _showDebug);
        
            if (!_attackLayers.Contains(other)) return;

            // Check if the other object has an EntityBase component
            if (!other.TryGetComponent(out GameplayEntityBase entity)) return;
        
            // Use the HitEntities collection to prevent multiple hits on the same entity
            // during the same attack.
            if (!_hitEntities.Add(entity)) return;

            _damageEffectType.ApplyEffect(entity);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject == gameObject) return;
        
            if (!_isAttacking) return;
        
            OCSFXLogger.Log($"{name} overlapped with {other.name}", this, _showDebug);
        
            if (!_attackLayers.Contains(other)) return;

            // Check if the other object has an EntityBase component
            if (!other.TryGetComponent(out GameplayEntityBase entity)) return;
        
            // Use the HitEntities collection to prevent multiple hits on the same entity
            // during the same attack.
            if (!_hitEntities.Add(entity)) return;

            _damageEffectType.ApplyEffect(entity);
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!_isAttacking) return;
        
            // Release the hit entities if they exit the attack collider
            if (other.TryGetComponent(out GameplayEntityBase entity))
            {
                _hitEntities.Remove(entity);
            }
        }

        private void Reset()
        {
            _attackCollider = GetComponent<CircleCollider2D>();
            _attackCollider.isTrigger = true;
        }
    
        private void HandleAttackWindow(float duration)
        {
            if (_attackWindowCoroutine != null)
            {
                StopCoroutine(_attackWindowCoroutine);
            }
            _attackWindowCoroutine = StartCoroutine(Co_HandleAttackWindow(duration));
        }
    
        private IEnumerator Co_HandleAttackWindow(float duration)
        {
            yield return GameOff2024Statics.GetWaitForSeconds(duration);
        
            if (!_isAttacking) yield break;
        
            EndAttack();
        }

        private void OnValidate()
        {
            if (!_attackCollider)
            {
                _attackCollider = GetComponent<CircleCollider2D>();
            }

            _attackCollider.isTrigger = true;
        }
    }
}
