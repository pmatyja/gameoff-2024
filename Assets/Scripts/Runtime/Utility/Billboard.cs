using UnityEngine;

namespace GameOff2024.Utility
{
    public class Billboard : MonoBehaviour
    {
        private static UnityEngine.Camera _mainCamera;
        
        private void Update()
        {
            // Rotate the object to be facing the same orientation as the camera
            transform.LookAt(transform.position + GetMainCamera().transform.rotation * Vector3.forward,
                GetMainCamera().transform.rotation * Vector3.up);
        }
        
        private static UnityEngine.Camera GetMainCamera()
        {
            if (!_mainCamera) _mainCamera = UnityEngine.Camera.main;

            return _mainCamera;
        }
    }
}