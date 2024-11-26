using System;
using UnityEngine;

public class AutoRotateBehavior : MonoBehaviour
{
    [Tooltip("Degrees per second, per axis")]
    [field: SerializeField] public Vector3 RotationSpeed { get; private set; } = new Vector3(0, 360, 0);
    [field: SerializeField] public Vector3 RandomStartRange { get; private set; } = Vector3.zero;

    private bool _rotate;
    private Vector3 _startRotation;

    private void OnEnable()
    {
        _rotate = true;
    }

    private void OnDisable()
    {
        _rotate = false;
        
        transform.rotation = Quaternion.Euler(_startRotation);
    }

    private void Start()
    {
        _startRotation = transform.rotation.eulerAngles;
        
        if (RandomStartRange != Vector3.zero)
        {
            transform.Rotate(new Vector3(
                UnityEngine.Random.Range(-RandomStartRange.x, RandomStartRange.x),
                UnityEngine.Random.Range(-RandomStartRange.y, RandomStartRange.y),
                UnityEngine.Random.Range(-RandomStartRange.z, RandomStartRange.z)
            ));
        }
        
        _rotate = true;
    }
    
    private void Update()
    {
        if (!_rotate) return;
        
        transform.Rotate(RotationSpeed * Time.deltaTime);
    }
}
