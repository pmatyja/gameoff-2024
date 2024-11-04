using UnityEngine;

namespace Runtime
{
    public class Billboard : MonoBehaviour
    {
        private static Camera _mainCamera;
        
        private void Update()
        {
            // Rotate the object to be facing the same orientation as the camera
            transform.LookAt(transform.position + GetMainCamera().transform.rotation * Vector3.forward,
                GetMainCamera().transform.rotation * Vector3.up);
        }
        
        private static Camera GetMainCamera()
        {
            if (!_mainCamera) _mainCamera = Camera.main;

            return _mainCamera;
        }
    }
}