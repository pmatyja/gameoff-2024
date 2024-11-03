using UnityEngine;
using UnityEngine.Events;

namespace Runtime
{
    public class PointerInteractableParent : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent OnClick { get; private set; } = new UnityEvent();
        
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
            Debug.Log("Clicked on a child of " + name, this);
            
            OnClick?.Invoke();
        }
        
        private void BindChildren(bool bind)
        {
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent<Collider>(out _))
                {
                    Debug.Log($"{child.name} (child of {name}) does not have a Collider component. Skipping.", this);
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