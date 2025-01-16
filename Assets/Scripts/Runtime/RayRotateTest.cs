using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class RayRotateTest : MonoBehaviour
{
    [SerializeField, Min(0.001f)] private float _updateRate = 0.5f;
 
    [Space]
    [SerializeField] private float _rotationAngle = 10f;
    [SerializeField] private Axis _rotationAxis = Axis.Y;
    
    [Space]
    [SerializeField] private Transform _rayStartPoint;
    [SerializeField] private float _rayLength = 10f;
    
    [Header("Debug")]
    [SerializeField] private bool _showDebug;
    [SerializeField] private bool _useCustomRayColor;
    [SerializeField] private Color _rayColor = Color.green;
    [FormerlySerializedAs("_useCustomRayDrawTime")] [SerializeField] private bool _useCustomDrawTime;
    [FormerlySerializedAs("_customRayDrawTime")] [SerializeField] private float _customDrawTime = 0.5f;
    private List<GameObject> _debugObjects = new List<GameObject>();
    
    private Vector3 _currentDirection;
    
    private float _updateTimer;
    private Quaternion _startRotation;
    private Vector3 _startDirection;

    private RaycastHit _lastHit;

    private void Awake()
    {
        _startRotation = transform.rotation;
        
        // Set the current direction to be perpendicular to the rotation axis
        _currentDirection = GetCrossProduct();
        
        _startDirection = _currentDirection;
    }

    private void Update()
    {
        if (_updateTimer >= _updateRate)
        {
            SetUpdatedRayRotation();
            _updateTimer = 0f;

            return;
        }
        
        _updateTimer += Time.deltaTime;
    }

    private void SetUpdatedRayRotation()
    {
        var rotation = Quaternion.AngleAxis(_rotationAngle, GetRotationAxisVector());
        _currentDirection = (rotation * _currentDirection).normalized;
        
        _lastHit = DoRaycast();

        if (_showDebug)
        {
            DrawDebugRay();
            
            if (_lastHit.collider)
            {
                DrawDebugHit(_lastHit.point, GetRainbowColor(), _useCustomDrawTime ? _customDrawTime : _updateRate);
            }
        }
    }

    private Transform GetRayStartPoint()
    {
        if (!_rayStartPoint) _rayStartPoint = transform;

        return _rayStartPoint;
    }
    
    private Vector3 GetRotationAxisVector()
    {   
        // use flags to get a combines normalized vector
        var axis = Vector3.zero;
        
        if ((_rotationAxis & Axis.X) == Axis.X)
        {
            axis += Vector3.right;
        }
        
        if ((_rotationAxis & Axis.Y) == Axis.Y)
        {
            axis += Vector3.up;
        }
        
        if ((_rotationAxis & Axis.Z) == Axis.Z)
        {
            axis += Vector3.forward;
        }
        
        return axis.normalized;
    }

    private Vector3 GetCrossProduct()
    {
        var comparisonVector = Vector3.up;
        
        if (Vector3.Dot(comparisonVector, GetRotationAxisVector()) > 0.99f)
        {
            comparisonVector = Vector3.right;
        }
        
        return Vector3.Cross(GetRotationAxisVector(), comparisonVector).normalized;
    }
    
    private Tuple<Vector3, Vector3> GetRayEndPoints()
    {
        var startPoint = GetRayStartPoint().position;
        var endPoint = _currentDirection * _rayLength;

        return new Tuple<Vector3, Vector3>(startPoint, endPoint);
    }
    
    private RaycastHit DoRaycast()
    {
        var (startPoint, endPoint) = GetRayEndPoints();

        if (Physics.Raycast(startPoint, endPoint.normalized, out var hit, _rayLength))
        {
            Debug.Log($"Hit {hit.collider.name} at {hit.point}");
        }
        
        return hit;
    }

    private void DrawDebugRay()
    {
        var rayDrawTime = _useCustomDrawTime ? _customDrawTime : _updateRate;
        
        // by default, cycle through the colors of the rainbow as a gradient
        var rayDrawColor = _useCustomRayColor ? _rayColor : GetRainbowColor();
        
        var (startPoint, endPoint) = GetRayEndPoints();
        
        Debug.DrawRay(startPoint, endPoint, rayDrawColor, rayDrawTime);
    }
    
    private void DrawDebugHit(Vector3 position, Color color, float duration)
    {
        StartCoroutine(Co_HandleDrawHit(position, color, duration));
    }
    
    private IEnumerator Co_HandleDrawHit(Vector3 position, Color color, float duration)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.1f;
        sphere.GetComponent<Renderer>().material.color = color;
        sphere.hideFlags = HideFlags.HideAndDontSave;
        
        _debugObjects.Add(sphere);
        var addedIndex = _debugObjects.Count - 1;
        
        yield return new WaitForSeconds(duration);

        if (!sphere)
        {
            _debugObjects.RemoveAt(addedIndex);
            yield break;
        }
        
        _debugObjects.Remove(sphere);
        Destroy(sphere);
    }
    
    private Color GetRainbowColor()
    {
        Color.RGBToHSV(_rayColor, out var h, out var s, out var v);
        
        // get the current angle compared to the start angle in clockwise degrees
        var angleFromStart = Vector3.SignedAngle(_startDirection, _currentDirection, GetRotationAxisVector());
        
        // normalize the angle to a 0-360 range
        if (angleFromStart < 0)
        {
            angleFromStart += 360;
        }
        
        h = angleFromStart / 360f;
        
        var returnColor = Color.HSVToRGB(h, s, v);

        return returnColor;
    }

    [ContextMenu(nameof(ResetRotation))]
    public void ResetRotation()
    {
        // Set the current direction to be perpendicular to the rotation axis
        _currentDirection = GetCrossProduct();
        
        _startDirection = _currentDirection;
    }
    
    [ContextMenu(nameof(ResetTimer))]
    public void ResetTimer() => _updateTimer = 0f;

    private void OnDestroy()
    {
        foreach (var obj in _debugObjects.Where(obj => obj))
        {
            Destroy(obj);
        }

        _debugObjects.Clear();
    }

    [Flags]
    private enum Axis
    {
        X = 1,
        Y = 2,
        Z = 4
    }
}
