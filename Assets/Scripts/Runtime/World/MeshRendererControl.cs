using System;
using UnityEngine;

namespace Runtime.World
{
    public class MeshRendererControl : MonoBehaviour
    {
        [SerializeField] private bool _enabled = true;
        [SerializeField] private bool _setOnAwake = true;
        
        private MeshRenderer[] _meshRenderers;
        
        private void Awake()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            
            if (_setOnAwake) SetMeshRenderersEnabled(_enabled);
        }

        public void SetMeshRenderersEnabled(bool enable)
        {
            _enabled = enable;
            
            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.enabled = _enabled;
            }
        }
        
        public void ToggleMeshRenderersEnabled()
        {
            _enabled = !_enabled;
            SetMeshRenderersEnabled(_enabled);
        }

        private void OnValidate()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            
            SetMeshRenderersEnabled(_enabled);
        }
    }
}
