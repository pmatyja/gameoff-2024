using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interactions
{
    // Derived from Lavgine Collectable
    public class GameOff2024Collectable : Collectable
    {
        [SerializeField, Expandable] private CollectableData _data;
        [field: SerializeField] public UnityEvent OnCollect { get; protected set; }

        protected override void OnPickUp(GameObject pickerObject)
        {
            OnCollect?.Invoke();
            
            if (_data) _data.OnCollect(transform);
            
            base.OnPickUp(pickerObject);
        }
    }
}