using OCSFX.Utility.Debug;
using UnityEngine;

namespace GameOff2024.Interactions
{
    public class InteractableDoor : MonoBehaviour, ICharacterInteractable
    {
        [field: SerializeField] public bool CanInteract { get; protected set; }
        [field: SerializeField] public bool IsRepeatable { get; protected set; }
        [field: SerializeField] public bool HasInteracted { get; protected set; }
        [field: SerializeField] public bool IsOpen { get; protected set; }
        
        [field: Space]
        [SerializeField] protected InteractionPrompt _interactionPrompt;

        [Header("Debug")]
        [SerializeField] protected bool _showDebug;
        
        public virtual void Interact()
        {
            if (HasInteracted && !IsRepeatable) return;
            if (!CanInteract) return;
            
            if (IsOpen) Close();
            else Open();
        }

        public virtual void ShowInteractionPrompt(bool show)
        {
            if (_interactionPrompt) _interactionPrompt.Show(show);
            
            OCSFXLogger.Log($"Show interaction prompt: {show}", this, _showDebug);
        }
        
        public void Unlock() => CanInteract = true;
        public void Lock() => CanInteract = false;
        
        protected virtual void Open()
        {
            // Open the door
            IsOpen = true;
            
            HasInteracted = true;
        }
        
        protected virtual void Close()
        {
            // Close the door
            IsOpen = false;
            
            HasInteracted = true;
        }
    }
}