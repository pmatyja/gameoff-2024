using System;

namespace Runtime.Utility
{
    [Flags]
    public enum MonoBehaviourFunction
    {
        None = 0,
        Awake = 1 << 0,
        Start = 1 << 1,
        OnEnable = 1 << 2,
        OnDisable = 1 << 3,
        OnDestroy = 1 << 4,
        OnTriggerEnter = 1 << 5,
        OnTriggerExit = 1 << 6,
        OnCollisionEnter = 1 << 7,
        OnCollisionExit = 1 << 8,
        OnTriggerEnter2D = 1 << 9,
        OnTriggerExit2D = 1 << 10,
        OnCollisionEnter2D = 1 << 11,
        OnCollisionExit2D = 1 << 12,
        OnMouseEnter = 1 << 13,
        OnMouseExit = 1 << 14,
        OnMouseOver = 1 << 15,
        OnMouseDown = 1 << 16,
        OnMouseUp = 1 << 17,
    }
}