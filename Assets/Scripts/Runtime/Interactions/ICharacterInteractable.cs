using UnityEngine;

namespace GameOff2024.Interactions
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