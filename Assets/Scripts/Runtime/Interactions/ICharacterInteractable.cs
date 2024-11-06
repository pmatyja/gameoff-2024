using UnityEngine;

namespace Runtime.Interactions
{
    public interface ICharacterInteractable
    {
        public void Interact();
        public void ShowInteractionPrompt(bool show);
        
        public bool CanInteract { get; }
        public bool IsRepeatable { get; }
        public bool HasInteracted { get; }
        public Transform transform { get; }
    }
}