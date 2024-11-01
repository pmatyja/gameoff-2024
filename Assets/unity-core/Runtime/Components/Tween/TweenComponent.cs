using UnityEngine;
using UnityEngine.Events;

public class TweenComponent : MonoBehaviour
{
    public bool Enabled = true;

    [SerializeField]
    private TweenCurve properties;

    [SerializeField]
    private UnityEvent<GameObject, float> OnUpdateCallback;

    [SerializeField]
    private UnityEvent<GameObject> OnCompleteCallback;

    public void Update()
    {
        if (this.Enabled == false)
        {
            return;
        }

        Tween.Interpolate
        (
            this.properties,
            (value) => this.OnUpdateCallback?.Invoke(this.gameObject, value),
            () => this.OnCompleteCallback?.Invoke(this.gameObject)
        );
    }
}
