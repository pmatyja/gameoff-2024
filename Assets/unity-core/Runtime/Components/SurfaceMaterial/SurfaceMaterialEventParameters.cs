using System;
using UnityEngine;

[Serializable]
public struct SurfaceMaterialEventParameters
{
    public GameObject Source;
    public PhysicsMaterial SurfaceMaterial;
}

[Serializable]
public struct SurfaceMaterial2DEventParameters
{
    public GameObject Source;
    public PhysicsMaterial2D SurfaceMaterial;
}