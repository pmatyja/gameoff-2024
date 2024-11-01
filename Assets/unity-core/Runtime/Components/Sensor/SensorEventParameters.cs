using System;
using UnityEngine;

[Serializable]
public struct SensorEventParameters
{
	public Sensor Sensor;
    public GameObject Target;
    public float DetectionFactor;
}
