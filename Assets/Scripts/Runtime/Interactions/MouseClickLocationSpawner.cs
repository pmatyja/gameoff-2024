using System.Collections;
using UnityEngine;

namespace Runtime.Interactions
{
    public class MouseClickLocationSpawner : SimplePrefabSpawner
    {
        [SerializeField] private bool _attachToDetectedSurface;
        
        [field: Header("Debug")]
        [SerializeField] private bool _showDebug;

        [SerializeField] private float _debugSphereRadius = 0.1f;
        [SerializeField] private float _debugSphereDuration = 0.5f;

        private Vector3 _clickedPosition;
        private Quaternion _clickedRotation;
        
        private bool _drawDebugSphere;
        private Coroutine _debugSphereCoroutine;

        private void OnMouseDown()
        {
            var mainCamera = GameOff2024Statics.GetMainCamera();
            if (!mainCamera) return;
            
            _clickedPosition = Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo)
                ? hitInfo.point
                : mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // get the rotation based on the normal of the hit
            _clickedRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            
            SpawnAt(_clickedPosition, _clickedRotation);

            if (_attachToDetectedSurface && hitInfo.transform)
            {
                var newInstance = GetInstances().Last();
                newInstance.transform.SetParent(hitInfo.transform);
            }
            
            if (_showDebug && Application.isPlaying) DrawDebugSphere();
        }
        
        public void SpawnAtMousePosition()
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