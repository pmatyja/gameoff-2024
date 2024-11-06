using System;
using UnityEngine;

namespace GameOff2024.Controller
{
    public abstract class GameOff2024ControllerBase : MonoBehaviour
    {
        public abstract Vector3 GetVelocity();
        public abstract Vector3 GetMovementVelocity();
        public abstract bool IsGrounded();
        
        public Action<Vector3> OnJump { get; protected set; }
        public Action<Vector3> OnLand { get; protected set; }
    }
}