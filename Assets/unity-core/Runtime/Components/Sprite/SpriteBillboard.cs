using UnityEngine;

[DisallowMultipleComponent]
public class SpriteBillboard : MonoBehaviour
{
    [SerializeField]
    private bool alwaysUpdate = true;

    [SerializeField]
    private bool axisAligned = true;

    [SerializeField]
    [Range(0.0f, 3600.0f)]
    private float rotationSpeed = 1080.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minMoveDistance = 0.01f;

    private Camera cameraObject;
    private Vector3 lastPosition;
    private float rotation = 0.0f;

    private void Awake()
    {
        this.cameraObject = Camera.main;

        if (this.axisAligned)
        {
            this.transform.eulerAngles = new Vector3(0.0f, this.cameraObject.transform.eulerAngles.y, 0.0f);
        }
        else
        {
            this.transform.eulerAngles = this.cameraObject.transform.eulerAngles;
        }
    }

    private void Update()
    {
        if (this.alwaysUpdate == false)
        {
            return;
        }

        if (this.cameraObject == null)
        {
            return;
        }

        var direction = (this.transform.position - this.lastPosition).normalized;
        var forward = this.cameraObject.transform.forward;

        forward.y = 0.0f;
        forward = forward.normalized;

        var facingDirection = Vector3.SignedAngle(forward, direction, Vector3.up);

        if (facingDirection < 0.0f)
        {
            this.rotation = 180.0f;
        }
        else if (facingDirection > 0.0f)
        {
            this.rotation = 0.0f;
        }

        var newRotation = this.cameraObject.transform.eulerAngles;

        if (this.axisAligned)
        {
            newRotation.x = 0;
            newRotation.z = 0;
        }

        newRotation.y = this.cameraObject.transform.eulerAngles.y + this.rotation;

        if (this.rotationSpeed > 0.0f)
        {
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(newRotation), Time.deltaTime * this.rotationSpeed);
        }
        else
        {
            this.transform.eulerAngles = newRotation;
        }

        var distance = Vector3.Distance(this.transform.position, this.lastPosition);
        if (distance > this.minMoveDistance)
        {
            this.lastPosition = this.transform.position;
        }
    }

    private void OnValidate()
    {
        this.cameraObject = Camera.main;

        if (this.cameraObject == null)
        {
            return;
        }

        if (this.axisAligned)
        {
            this.transform.eulerAngles = new Vector3(0.0f, this.cameraObject.transform.eulerAngles.y, 0.0f);
        }
        else
        {
            this.transform.eulerAngles = this.cameraObject.transform.eulerAngles;
        }
    }
}
