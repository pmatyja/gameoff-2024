using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteAnimationPlayer))]
public class SpriteAnimationSelector : MonoBehaviour
{
    [SerializeField]
    public string onMove;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float moveThreshold = 0.01f;

    [SerializeField]
    private bool interuptAnimation;

    private SpriteRenderer spriteRenderer;
    private SpriteAnimationPlayer animationPlayer;
    private Vector3 lastPosition;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.animationPlayer = this.GetComponent<SpriteAnimationPlayer>();
    }

    private void Update()
    {
        var distance = Vector3.Distance(this.transform.position, this.lastPosition);
        if (distance > this.moveThreshold)
        {
            if (string.IsNullOrWhiteSpace(this.onMove) == false)
            {
                this.animationPlayer.Play(this.onMove);
            }

            this.lastPosition = this.transform.position;
        }
        else
        {
            if (this.interuptAnimation)
            {
                this.animationPlayer.NextAnimation();
            }
        }
    }

    private void OnValidate()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
}
