using Runtime.Utility;
using UnityEngine;


public class Destination : MonoBehaviour
{
    private static readonly float _positionRadius = 0.5f;
    private static readonly float _forwardRadius = 0.1f;
    
    public void SetPositionAndRotation(Transform target)
    {
        target.SetPositionAndRotation(transform.position, transform.rotation);
    }
    
    public void SetPosition(Transform target)
    {
        target.position = transform.position;
    }
    
    public void SetRotation(Transform target)
    {
        target.rotation = transform.rotation;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        
        Gizmos.DrawWireSphere(transform.position, _positionRadius);

        Gizmos.color *= 0.25f;
        Gizmos.DrawSphere(transform.position, _positionRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        
        Gizmos.DrawWireSphere(transform.position + transform.forward, _forwardRadius);
        
        Gizmos.color *= 0.25f;
        Gizmos.DrawSphere(transform.position + transform.forward, _forwardRadius);
    }
}
