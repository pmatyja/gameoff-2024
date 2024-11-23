using System;
using UnityEngine;
using Runtime.Interactions;

namespace Runtime.Collectables
{
    public class CharacterInteractionBehavior : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 1f;
        [SerializeField] private int _overlapCountLimit = 4;
        private Collider[] _overlapResults;
        private ICharacterInteractable _targetInteractable;
        private bool CanInteract => _targetInteractable != null;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        private void Awake()
        {
            _overlapResults = new Collider[_overlapCountLimit];
        }

        private void OnEnable()
        {
            InputHandler.Get().OnGameplayInteractInput += OnGameplayInteractInput;
        }

        private void OnDisable()
        {
            InputHandler.Get().OnGameplayInteractInput -= OnGameplayInteractInput;
        }

        private void FixedUpdate()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, _interactionRadius, _overlapResults);

            ICharacterInteractable newTargetInteractable = null;
            for (var i = 0; i < size; i++)
            {
                var interactable = _overlapResults[i].GetComponent<ICharacterInteractable>();
                if (interactable is not { CanInteract: true }) continue;
                
                if (newTargetInteractable == null || 
                    Vector3.Distance(transform.position, interactable.transform.position) < 
                    Vector3.Distance(transform.position, newTargetInteractable.transform.position))
                {
                    newTargetInteractable = interactable;
                }
            }

            if (_targetInteractable != newTargetInteractable)
            {
                _targetInteractable?.ShowInteractionPrompt(false);
            }

            _targetInteractable = newTargetInteractable;
            _targetInteractable?.ShowInteractionPrompt(true);
        }
        
        private void OnGameplayInteractInput()
        {
            if (!CanInteract) return;
            
            _targetInteractable.Interact();
        }

        private void OnValidate()
        {
            if (_overlapResults == null || _overlapResults.Length != _overlapCountLimit)
            {
                _overlapResults = new Collider[_overlapCountLimit];
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_showDebug) return;
            
            Gizmos.color = CanInteract ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, _interactionRadius);
        }
    }
}