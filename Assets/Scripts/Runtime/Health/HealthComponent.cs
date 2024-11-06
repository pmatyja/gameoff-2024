using System;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace GameOff2024.Health
{
    public class HealthComponent : MonoBehaviour, IDamagable
    {
        public const int DEFAULT_MAX_HEALTH = 100;
    
        [SerializeField, Min (0)] private int _currentHealth;
        [SerializeField, Min (1)] private int _maxHealth = DEFAULT_MAX_HEALTH;
    
        private int _cachedAmount;
    
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        [SerializeField] private bool _isInvincible;
    
        [Header("Events")]
        [SerializeField] private UnityEvent<int> _onHealed;
        [SerializeField] private UnityEvent<int> _onDamaged;
        [SerializeField] private UnityEvent _onDied;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        public float HealthPercentage => (float)_currentHealth / _maxHealth;
    
        public bool IsDead() => _currentHealth <= 0;

        public event Action<int> Healed;
        public event Action<int> Damaged;
        public event Action Died;
        public event Action<int> ValueSet;

        public bool TakeDamage(int amount)
        {
            if (IsDead())
            {
                OCSFXLogger.Log($"Cannot damage {name}. Already dead.", this, _showDebug);
                return false;
            }

            if (!TryRemoveHealth(amount, out var amountRemoved))
            {
                if (_isInvincible)
                {
                    OCSFXLogger.Log($"Cannot damage {name}. Invincible.", this, _showDebug);
                    return false;
                }
            
                OCSFXLogger.Log($"Cannot damage {name}. Already dead.", this, _showDebug);
                return false;
            }
        
            OCSFXLogger.Log($"Dealt {amountRemoved} damage. Current health: {_currentHealth}", this, _showDebug);
        
            Damaged?.Invoke(amountRemoved);
            _onDamaged?.Invoke(amountRemoved);

            if (!IsDead()) return true;
        
            OCSFXLogger.Log($"{name} has died.", this, _showDebug);
            Died?.Invoke();
            _onDied?.Invoke();

            return true;
        }

        public bool Heal(int amount)
        {
            if (IsDead())
            {
                OCSFXLogger.Log($"Cannot heal {name}. Already dead.", this, _showDebug);
                return false;
            }

            if (!TryAddHealth(amount, out var amountAdded))
            {
                OCSFXLogger.Log($"Cannot heal {name}. Already full.", this, _showDebug);
                return false;
            }
        
            OCSFXLogger.Log($"Healed {amountAdded} health. Current health: {_currentHealth}", this, _showDebug);
        
            Healed?.Invoke(amountAdded);
            _onHealed?.Invoke(amountAdded);
        
            return true;
        }

        [ContextMenu(nameof(SetFullHealth))]
        public bool SetFullHealth()
        {
            ValueSet?.Invoke(_maxHealth);
            return TryAddHealth(_maxHealth, out _);
        }
    
        [ContextMenu(nameof(Kill))]
        public void Kill()
        {
            if (IsDead()) return;

            TakeDamage(_maxHealth);
        }
    
#if UNITY_EDITOR
    [ContextMenu(nameof(EditorTakeDamageOneQuarter))] 
    public void EditorTakeDamageOneQuarter() => TakeDamage(_maxHealth / 4); 
#endif

        public void SetCurrentHealth(int amount)
        {
            _currentHealth = Mathf.Clamp(amount, 0, _maxHealth);
            ValueSet?.Invoke(_currentHealth);
        }

        public void SetMaxHealth(int amount)
        {
            _maxHealth = Mathf.Max(1, amount);
        }
    
        private bool TryAddHealth(int amountSent, out int amountAdded)
        {
            if (IsDead())
            {
                amountAdded = 0;
                return false;
            }

            amountAdded = Mathf.Min(_maxHealth - _currentHealth, amountSent);
            _currentHealth += amountAdded;

            return true;
        }
    
        private bool TryRemoveHealth(int amountSent, out int amountRemoved)
        {
            if (_isInvincible)
            {
                amountRemoved = 0;
                return false;
            }
        
            if (IsDead())
            {
                amountRemoved = 0;
                return false;
            }

            amountRemoved = Mathf.Min(_currentHealth, amountSent);
            _currentHealth -= amountRemoved;

            return true;
        }
    
        private void OnValidate()
        {
            if (_cachedAmount == _currentHealth) return;
            var delta = _currentHealth - _cachedAmount;
        
            switch (delta)
            {
                case > 0:
                    Healed?.Invoke(delta);
                    _onHealed?.Invoke(delta);
                    break;
                case < 0:
                    Damaged?.Invoke(delta);
                    _onDamaged?.Invoke(delta);
                    break;
            }
            
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
            _cachedAmount = _currentHealth;
        }

        private void Reset()
        {
            _currentHealth = _maxHealth;
        }
    }
}
