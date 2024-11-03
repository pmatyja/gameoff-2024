using UnityEngine;
using UnityEngine.Events;

public class PointerInteractable : MonoBehaviour
{
    [field: SerializeField] public UnityEvent OnClick { get; set; }
    
    private void OnMouseDown()
    {
        Debug.Log("Clicked on " + name, this);
        
        OnClick.Invoke();
    }
}
