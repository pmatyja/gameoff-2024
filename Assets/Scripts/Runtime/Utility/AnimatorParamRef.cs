using System;
using UnityEngine;

namespace Runtime.Utility
{
    [Serializable]
    public class AnimatorParamRef
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _name;
        [SerializeField] private AnimatorControllerParameterType _type;
        
        public Animator Animator => _animator;
        public string Name => _name;
        public AnimatorControllerParameterType Type => _type;
        
        public int Hash { get; private set; }

        public AnimatorParamRef(string name, AnimatorControllerParameterType type = AnimatorControllerParameterType.Bool)
        {
            _name = name;
            _type = type;
            Hash = Animator.StringToHash(name);
        }
        
        public AnimatorParamRef(Animator animator, string name, AnimatorControllerParameterType type = AnimatorControllerParameterType.Bool)
        {
            _animator = animator;
            _name = name;
            _type = type;
            Hash = Animator.StringToHash(name);
        }
        
        public void SetValue(Animator animator, object value)
        {
            HandleIntFloatCast(ref value);

            if (!IsTypeCompatible(value)) return;
            
            switch (value)
            {
                case bool boolValue:
                    SetValue(animator, boolValue);
                    break;
                case float floatValue:
                    SetValue(animator, floatValue);
                    break;
                case int intValue:
                    SetValue(animator, intValue);
                    break;
                default:
                    throw new ArgumentException(
                        $"[{nameof(AnimatorParamRef)}] {nameof(SetValue)} for type {value.GetType()} is not supported." +
                        $"Must be of type bool, float, or int.");
            }
        }
        
        public void SetValue(object value)
        {
            if (_animator == null)
            {
                Debug.LogError($"[{nameof(AnimatorParamRef)}] {nameof(SetValue)} cannot be called without an {nameof(Animator)} reference."
                               + $"One must be provided in the constructor or passed into the other signature of this method.");
                return;
            }
            
            SetValue(_animator, value);
        }

        private void HandleIntFloatCast(ref object value)
        {
            if (value is int asIntValue && _type == AnimatorControllerParameterType.Float)
            {
                value = (float)asIntValue;
            }
        }

        private bool IsTypeCompatible(object value)
        {
            var valueType = value.GetType();
            var isCompatible =  valueType == typeof(bool) && _type == AnimatorControllerParameterType.Bool ||
                   valueType == typeof(float) && _type == AnimatorControllerParameterType.Float ||
                   valueType == typeof(int) && _type == AnimatorControllerParameterType.Int;

            if (!isCompatible) LogIncorrectTypeError(value);
            
            return isCompatible;
        }
        
        private void LogIncorrectTypeError(object value)
        {
            Debug.LogError($"[{nameof(AnimatorParamRef)}] {value.GetType()} is incompatible with {nameof(AnimatorParamRef)}({_type})." +
                           $"Expected type {_type}.");
        }
        
        private void SetValue(Animator animator, float value) => animator.SetFloat(Hash, value);
        private void SetValue(Animator animator, bool value) => animator.SetBool(Hash, value);
        private void SetValue(Animator animator, int value) => animator.SetInteger(Hash, value);

        public void SetTrigger(Animator animator)
        {
            if (_type != AnimatorControllerParameterType.Trigger)
            {
                Debug.LogError($"[{nameof(AnimatorParamRef)}] Cannot {nameof(SetTrigger)} for {nameof(AnimatorParamRef)}({_type})." +
                                $"Expected type {AnimatorControllerParameterType.Trigger}.");
                return;
            }
            
            animator.SetTrigger(Hash);
        }

        public void ResetTrigger(Animator animator)
        {
            if (_type != AnimatorControllerParameterType.Trigger)
            {
                Debug.LogError($"[{nameof(AnimatorParamRef)}] Cannot {nameof(ResetTrigger)} for {nameof(AnimatorParamRef)}({_type})." +
                                $"Expected type {AnimatorControllerParameterType.Trigger}.");
                return;
            }
            
            animator.ResetTrigger(Hash);
        }
    }
}