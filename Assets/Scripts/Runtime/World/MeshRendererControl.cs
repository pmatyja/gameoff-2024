using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.World
{
    public class MeshRendererControl : MonoBehaviour
    {
        [FormerlySerializedAs("_enabled")] [SerializeField] private bool _currentState = true;
        [FormerlySerializedAs("_setOnAwake")] [SerializeField] private bool _stateOnAwake = true;
        
        private MeshRenderer[] _meshRenderers;
        
        private void Awake()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            
            SetMeshRenderersEnabled(_stateOnAwake);
        }

        public void SetMeshRenderersEnabled(bool enable)
        {
            _currentState = enable;
            
            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.enabled = _currentState;
            }
        }
        
        public void ToggleMeshRenderersEnabled()
        {
            _currentState = !_currentState;
            SetMeshRenderersEnabled(_currentState);
        }

        private void OnValidate()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            
            SetMeshRenderersEnabled(_currentState);
        }
    }
}
