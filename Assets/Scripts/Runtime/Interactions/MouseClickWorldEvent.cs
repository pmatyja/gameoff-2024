using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interactions
{
    public class MouseClickWorldEvent : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<Vector3, Quaternion> OnMouseClickPositionRotation { get; private set; }
        [field: SerializeField] public UnityEvent<Vector3> OnMouseClickPosition { get; private set; }
        [field: SerializeField] public UnityEvent OnMouseClick { get; private set; }
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        [SerializeField] private float _debugSphereRadius = 0.1f;
        [SerializeField] private float _debugSphereDuration = 0.5f;
        
        private Vector3 _clickedPosition;
        private Quaternion _clickedRotation;
        
        private Coroutine _debugSphereCoroutine;
        private bool _drawDebugSphere;
        
        private void OnMouseDown()
        {
            var mainCamera = GameOff2024Statics.GetMainCamera();
            if (!mainCamera) return;
            
            _clickedPosition = Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo)
                ? hitInfo.point
                : mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // get the rotation based on the normal of the hit
            _clickedRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            
            OnMouseClickPositionRotation?.Invoke(_clickedPosition, _clickedRotation);
            OnMouseClickPosition?.Invoke(_clickedPosition);
            OnMouseClick?.Invoke();
            
            if (_showDebug) DrawDebugSphere();
        }

        public void Invoke()
        {
            OnMouseDown();
        }
        
        private void OnDrawGizmos()
        {
            if (!_showDebug || !_drawDebugSphere) return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_clickedPosition, _debugSphereRadius);
        }

        private void DrawDebugSphere()
        {
            if (_debugSphereCoroutine != null) StopCoroutine(_debugSphereCoroutine);
            
            _debugSphereCoroutine = StartCoroutine(Co_HandleDrawDebugSphere());
        }

        private IEnumerator Co_HandleDrawDebugSphere()
        {
            _drawDebugSphere = true;
            
            var drawTime = 0f;
            
            while (drawTime < _debugSphereDuration)
            {
                drawTime += Time.deltaTime;
                yield return null;
            }
            
            _drawDebugSphere = false;
        }
    }
}