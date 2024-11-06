using System.Collections;
using OCSFX.Attributes;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2024.Health
{
    [RequireComponent(typeof(Image))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private Image _fillBar;
        [SerializeField] private Gradient _valueGradient;
        [SerializeField] private float _lowHealthThreshold = 0.25f;
        [SerializeField] private Color _lowHealthFlashColor = Color.white;
        [SerializeField] private float _lowHealthPulseRate = 1f;

        [field: SerializeField, ReadOnly, Range(0f, 1f)]
        public float Current { get; private set; }

        [SerializeField] private float _damageInterpSpeed = 5f;
        [SerializeField] private float _healInterpSpeed = 5f;
    
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
    
        private Coroutine _interpolateFillAmountCoroutine;
        private Coroutine _lowHealthPulseCoroutine;
    
        private void Awake()
        {
            if (!_healthComponent)
            {
                OCSFXLogger.LogError("HealthComponent not set.", this, _showDebug);
                return;
            }
        
            if (!_fillBar)
            {
                OCSFXLogger.LogError("FillBar not set.", this, _showDebug);
                return;
            }
        }
    
        private void Start() => UpdateFillBar();
    
        private void OnEnable()
        {
            _healthComponent.Healed += OnHealed;
            _healthComponent.Damaged += OnDamaged;
            _healthComponent.Died += OnDied;

            _healthComponent.ValueSet += OnValueSet;
        }
    
        private void OnDisable()
        {
            _healthComponent.Healed -= OnHealed;
            _healthComponent.Damaged -= OnDamaged;
            _healthComponent.Died -= OnDied;
        
            _healthComponent.ValueSet -= OnValueSet;
        }

        private void OnValueSet(int value) => UpdateFillBar();
        private void OnHealed(int amount) => UpdateFillBar();
        private void OnDamaged(int amount) => UpdateFillBar();
        private void OnDied() => UpdateFillBar();
    
        private void UpdateFillBar()
        {
            if (!_fillBar)
            {
                OCSFXLogger.LogError($"{typeof(Image)} not set on {name}.", this, _showDebug);
                return;
            }
        
            if (!_healthComponent)
            {
                OCSFXLogger.LogError($"{typeof(HealthComponent)} not set on {name}.", this, _showDebug);
                return;
            }
        
            OCSFXLogger.Log($"Updated fill bar to {_fillBar.fillAmount}.", this, _showDebug);
        
            Current = _healthComponent.HealthPercentage;
            AnimateFillBar(Current);
        }
    
        private void AnimateFillBar(float targetFillAmount)
        {
            if (_interpolateFillAmountCoroutine != null)
            {
                StopCoroutine(_interpolateFillAmountCoroutine);
            }
        
            _interpolateFillAmountCoroutine = 
                StartCoroutine(Co_InterpolateFillAmount(targetFillAmount));
        }
    
        private IEnumerator Co_InterpolateFillAmount(float targetFillAmount)
        {
            var currentFillAmount = _fillBar.fillAmount;
            var deltaSign = Mathf.Sign(targetFillAmount - currentFillAmount);
            var interpSpeed = deltaSign > 0 ? _healInterpSpeed : _damageInterpSpeed;
        
            while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * interpSpeed);
                _fillBar.fillAmount = currentFillAmount;

                HandleLowHealthPulseAnimation();
            
                yield return null;
            }
        }
    
        private bool HandleLowHealthPulseAnimation()
        {
            if (IsLowHealth())
            {
                if (_lowHealthPulseCoroutine == null)
                {
                    _lowHealthPulseCoroutine = StartCoroutine(Co_LowHealthPulse());
                }
                return true;
            }

            if (_lowHealthPulseCoroutine == null) return false;
        
            StopCoroutine(_lowHealthPulseCoroutine);
            _lowHealthPulseCoroutine = null;
            _fillBar.color = _valueGradient.Evaluate(_fillBar.fillAmount);

            return false;
        }
    
        private IEnumerator Co_LowHealthPulse()
        {
            var baseColor = _fillBar.color;
            var animate = IsLowHealth();
        
            while (animate)
            {
                _fillBar.color = Color.Lerp(baseColor, _lowHealthFlashColor, 
                    Mathf.PingPong(Time.time, 1/_lowHealthPulseRate));
            
                baseColor = _valueGradient.Evaluate(_fillBar.fillAmount);
                animate = IsLowHealth();
                yield return null;
            }
            while (_fillBar.color != baseColor)
            {
                _fillBar.color = Color.Lerp(_fillBar.color, baseColor, Time.deltaTime * 5f);
                yield return null;
            }
        
            _fillBar.color = _valueGradient.Evaluate(_fillBar.fillAmount);
            _lowHealthPulseCoroutine = null;
        }
    
        private bool IsLowHealth() => _fillBar.fillAmount <= _lowHealthThreshold;

        private void Reset()
        {
            _fillBar = GetComponent<Image>();
        
            _fillBar.type = Image.Type.Filled;
        }

        private void OnValidate()
        {
            Current = _fillBar.fillAmount;
            _fillBar.color = _valueGradient.Evaluate(_fillBar.fillAmount);
        }
    }
}
