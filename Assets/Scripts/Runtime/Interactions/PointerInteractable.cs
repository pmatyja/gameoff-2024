using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interactions
{
    public class PointerInteractable : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent OnClick { get; private set; } = new UnityEvent();
    
        private void OnMouseDown() => OnClick?.Invoke();
    }
}
