using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CollidersEventParameters
{
    public GameObject Source;
    public IReadOnlyCollection<Collider> Colliders;
}