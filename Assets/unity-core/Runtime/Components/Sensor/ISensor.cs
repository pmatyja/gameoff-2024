using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Sensor : MonoBehaviour
{
    public abstract IReadOnlyDictionary<GameObject, TrackedTarget> Targets { get; }
}
