using System;
using UnityEngine;

namespace Runtime
{
    public class CharacterInteractionBehavior : MonoBehaviour
    {
        [SerializeField] private int _overlapCountLimit = 10;
        private Collider[] _overlapResults;
        private ICharacterInteractable _targetInteractable;
        
        private void Awake()
        {
            _overlapResults = new Collider[_overlapCountLimit];
        }
        
        private void Update()
        {
            // Do a sphere overlap check for ICharacterInteractable objects
            // If the object is interactable, cache it for interaction
            // If there are multiple in the sphere, prioritize the closest one
            
            // If the player presses the interact button, call the Interact method on the cached object

            var size = Physics.OverlapSphereNonAlloc(transform.position, 1f, _overlapResults);
            
            for (var i = 0; i < size; i++)
            {
                // filter out non-interactable objects, then prioritize the closest one
                var interactable = _overlapResults[i].GetComponent<ICharacterInteractable>();
                if (interactable == null || !interactable.CanInteract) continue;
                
                if (_targetInteractable == null)
                {
                    _targetInteractable = interactable;
                }
                else
                {
                    var currentDistance = Vector3.Distance(transform.position, _targetInteractable.transform.position);
                    var newDistance = Vector3.Distance(transform.position, interactable.transform.position);
                    
                    if (newDistance < currentDistance)
                    {
                        _targetInteractable = interactable;
                    }
                }
            }
        }

        private void OnValidate()
        {
            if (_overlapResults == null || _overlapResults.Length != _overlapCountLimit)
            {
                _overlapResults = new Collider[_overlapCountLimit];
            }
        }
    }
}