using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDirection : MonoBehaviour
{
    [SerializeField]
    public int Direction
    {
        get
        {
            if (this.spriteRenderer == default)
            {
                return 1;
            }

            if (this.flipDirection)
            {
                return this.spriteRenderer.flipX == true ? 1 : -1;
            }

            return this.spriteRenderer.flipX == false ? 1 : -1;
        }
    }

    [SerializeField]
    private bool flipDirection;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float threshold = 0.01f;

    private SpriteRenderer spriteRenderer;
    private float lastPosition;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
// BEGIN_DIVERGENCE | owen | initialize player direction so it doesn't flip on start
        lastPosition = this.transform.position.x;
// END_DIVERGENCE | owen
    }

    private void Update()
    {
        var direction = this.transform.position.x - this.lastPosition;
        
        if (direction > this.threshold)
        {
            if (this.flipDirection)
            {
                this.spriteRenderer.flipX = true;
            }
            else
            {
                this.spriteRenderer.flipX = false;
            }

            this.lastPosition = this.transform.position.x;
        }
        else if (direction < -this.threshold)
        {
            if (this.flipDirection)
            {
                this.spriteRenderer.flipX = false;
            }
            else
            {
                this.spriteRenderer.flipX = true;
            }

            this.lastPosition = this.transform.position.x;
        }
    }

    private void OnValidate()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
}
