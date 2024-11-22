using System.Collections.Generic;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Runtime.Interactions
{
    public class PointerInteractableParent : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent OnClick { get; private set; } = new UnityEvent();
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        private void OnEnable()
        {
            BindChildren(true);
        }

        private void OnDisable()
        {
            BindChildren(false);
        }

        private void OnChildClicked()
        {
            if (UIHoverDetector.Hovering)
            {
                return;
            }

            Debug.Log("Clicked on a child of " + name, this);
            
            OnClick?.Invoke();
        }
        
        private void BindChildren(bool bind)
        {
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent<Collider>(out _))
                {
                    OCSFXLogger.Log($"{child.name} (child of {name}) does not have a Collider component. Skipping.", this, _showDebug);
                    continue;
                }

                var interactable = child.GetComponent<PointerInteractable>();
                if (bind)
                {
                    interactable = interactable ?? child.gameObject.AddComponent<PointerInteractable>();
                    interactable.OnClick.AddListener(OnChildClicked);
                }
                else if (interactable)
                {
                    interactable.OnClick.RemoveListener(OnChildClicked);
                }
            }
        }
    }
}