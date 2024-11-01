using UnityEngine;

public class ParalaxEffect : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float scale;

    private Camera cameraObject;
    private Vector3 lastCameraPosition;

    private void Awake()
    {
        if (this.cameraObject == null)
        {
            this.cameraObject       = UnityEngine.GameObject.FindObjectOfType<Camera>();
            this.lastCameraPosition = this.cameraObject.transform.position;

        }
    }

    private void Update()
    {
        if (this.cameraObject == null)
        {
            return;
        }

        var distanceX = this.cameraObject.transform.position.x - this.lastCameraPosition.x;

        if (Mathf.Abs(distanceX) < 0.01f)
        {
            distanceX = 0.0f;
        }

        var distanceY = this.cameraObject.transform.position.y - this.lastCameraPosition.y;

        if (Mathf.Abs(distanceY) < 0.01f)
        {
            distanceY = 0.0f;
        }

        if (Mathf.Abs(distanceX) > 0.0f || Mathf.Abs(distanceY) > 0.0f)
        {
            this.transform.position -= new Vector3(distanceX * this.scale, distanceY * this.scale, 0.0f);
            this.lastCameraPosition  = this.cameraObject.transform.position;
        }
    }
}
