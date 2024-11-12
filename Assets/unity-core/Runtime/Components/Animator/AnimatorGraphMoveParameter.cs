using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class AnimatorGraphParametersSource : MonoBehaviour
{
    [SerializeField]
    [Readonly]
    private Animator controller;

    [Header("Velocity")]

    [SerializeField]
    private Vector3 moveThreshold = new Vector3(0.01f, 0.01f, 0.01f);

    private Vector3 lastPosition;

    private static readonly int VelocityX = Animator.StringToHash("velocityX");
    private static readonly int VelocityY = Animator.StringToHash("velocityY");
    private static readonly int VelocityZ = Animator.StringToHash("velocityZ");

    private void Awake()
    {
        this.controller = this.GetComponent<Animator>();
    }

    private void Update()
    {
        var velocty = (this.transform.position - this.lastPosition);

        velocty.x = Math.Abs(velocty.x);
        velocty.y = Math.Abs(velocty.y);
        velocty.z = Math.Abs(velocty.z);

        // Velocity X

        if (velocty.x > this.moveThreshold.x)
        {
            this.controller.SetFloat(VelocityX, velocty.x);
            this.lastPosition.x = this.transform.position.x;
        }
        else
        {
            this.controller.SetFloat(VelocityX, 0.0f);
        }

        // Velocity Y
        
        if (velocty.y > this.moveThreshold.y)
        {
            this.controller.SetFloat(VelocityY, velocty.y);
            this.lastPosition.y = this.transform.position.y;
        }
        else
        {
            this.controller.SetFloat(VelocityY, 0.0f);
        }

        // Velocity Z

        if (velocty.z > this.moveThreshold.z)
        {
            this.controller.SetFloat(VelocityZ, velocty.z);
            this.lastPosition.z = this.transform.position.z;
        }
        else
        {
            this.controller.SetFloat(VelocityZ, 0.0f);
        }
    }

    private void OnValidate()
    {
        this.moveThreshold = Vector3.Max(this.moveThreshold, Vector3.zero);
    }
}