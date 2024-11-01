using System.Collections;
using UnityEngine;

public class FractureComponent : MonoBehaviour
{
    public void Animate(float destroyDelay, float animationDuration = 0.5f)
    {
        this.StartCoroutine(this.AnimateFracture(destroyDelay, animationDuration));
    }

    private IEnumerator AnimateFracture(float destroyDelay, float animationDuration)
    {
        // Wait
        yield return Wait.Seconds(destroyDelay + Random.Range(0.0f, 0.1f));

        // Scale down
        yield return this.StartCoroutine(Tween.Once(animationDuration, 0.0f, t => this.transform.localScale = Vector3.one * t));

        UnityEngine.GameObject.Destroy(this.gameObject);
    }
}