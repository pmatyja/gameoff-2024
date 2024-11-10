using System;
using UnityEngine;

public class AutoRotateBehavior : MonoBehaviour
{
    [Tooltip("Degrees per second, per axis")]
    [field: SerializeField] public Vector3 RotationSpeed { get; private set; } = new Vector3(0, 360, 0);

    private bool _rotate;
    
    private Vector3 _startRotation;

    private void OnDisable()
    {
        _rotate = false;
        
        transform.rotation = Quaternion.Euler(_startRotation);
    }

    private void Start()
    {
        _startRotation = transform.rotation.eulerAngles;
        _rotate = true;
    }
    
    private void Update()
    {
        if (!_rotate) return;
        
        transform.Rotate(RotationSpeed * Time.deltaTime);
    }
}
