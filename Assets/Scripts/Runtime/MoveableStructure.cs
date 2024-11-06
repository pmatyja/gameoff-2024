using System.Collections.Generic;
using OCSFX.Utility.Debug;
using UnityEngine;

namespace Runtime
{
    public class MoveableStructure : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        private readonly Dictionary<Transform /*child*/, Transform /*parent*/> _attachedChildren 
            = new Dictionary<Transform, Transform>();

        public void Attach(Transform child)
        {
            _attachedChildren.Add(child, child.parent);
            
            child.SetParent(transform);
            
            OCSFXLogger.Log($"Attached {child.name} to {name}", this, _showDebug);
        }
        
        public void Detach(Transform child)
        {
            if (!_attachedChildren.TryGetValue(child, out var attachedChild))
            {
                OCSFXLogger.LogWarning($"{child.name} is not attached to {name}", this, _showDebug);
                return;
            }
            
            child.SetParent(attachedChild);
            
            _attachedChildren.Remove(child);
            
            OCSFXLogger.Log($"Detached {child.name} from {name}", this, _showDebug);
        }
    }
}