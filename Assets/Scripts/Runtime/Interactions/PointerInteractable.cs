using UnityEngine;
using UnityEngine.Events;

namespace GameOff2024.Interactions
{
    public class PointerInteractable : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent OnClick { get; private set; } = new UnityEvent();
    
        private void OnMouseDown() => OnClick?.Invoke();
    }
}
