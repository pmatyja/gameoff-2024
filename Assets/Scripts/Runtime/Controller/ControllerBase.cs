using System;
using UnityEngine;

namespace Runtime.Controller
{
    public abstract class ControllerBase : MonoBehaviour
    {
        public abstract Vector3 GetVelocity();
        public abstract Vector3 GetMovementVelocity();
        public abstract bool IsGrounded();
        
        public event Action<Vector3> OnJump;
        public event Action<Vector3> OnLand;
    }
}