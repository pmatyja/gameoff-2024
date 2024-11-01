using UnityEngine;

public class Prompt : MonoBehaviour
{
    [SerializeField]
    [InputActionMap]
    private string source;
    public string Source => this.source;

    [SerializeField]
    [Readonly]
    private bool isNear;
    public bool IsNear => this.isNear;

    [SerializeField]
    private Vector3 targetPosition = new(0.0f, 1.0f, 0.0f);

    [SerializeField]
    private Vector3 targetScale = Vector3.one;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float animationDuration = 0.15f;

    private SpriteRenderer spriteRenderer;
    private float progress;
    private float target;

    public void Show()
    {
        this.target = 1.0f;
    }

    public void Hide()
    {
        this.target = 0.0f;
    }

    private void Awake()
    {
        this.target = 0.0f;
        this.progress = 1.0f;
    }

    private void Update()
    {
        this.progress = Tween.Interpolate(this.progress, this.animationDuration, target: this.target, onUpdate: value =>
        {
            this.transform.localPosition = Vector3.Lerp(Vector3.zero, this.targetPosition, value);
            this.transform.localScale = Vector3.Lerp(Vector3.zero, this.targetScale, value);
        });
    }
}
