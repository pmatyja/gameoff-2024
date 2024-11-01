using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Serializable]
public class RollingCredits : UiElement
{
    [Header("Input")]

    [SerializeField]
    [InputActionMap]
    private string inputSkip;

    [SerializeField]
    private AudioResourceSO music;

    [SerializeField]
    [Range(0.0f, 10000.0f)]
    private float endPosition = 1610.0f;

    [SerializeField]
    [Range(0.0f, 250.0f)]
    private float speed = 50.0f;

    private float startPosition;
    private VisualElement content;

    public override bool IsVisible(GameMode mode)
    {
        return mode == GameMode.Menu;
    }

    public override void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        base.Enable(behaviour, root.Q<VisualElement>(nameof(RollingCredits)));
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override IEnumerator Animate()
    {
        this.music?.Play();

        var sceneRequested = false;

        while (this.Behaviour.isActiveAndEnabled)
        {
            if (sceneRequested == false)
            {
                if (InputManager.Released(this.inputSkip))
                {
                    sceneRequested = true;
                    SceneManager.LoadSceneAsync(0);
                }
            }

            var newValue = Mathf.MoveTowards(this.Root.style.top.value.value, -this.endPosition, this.speed * Time.deltaTime);

            this.Root.style.top = new Length(newValue, LengthUnit.Pixel);

            yield return base.Animate();
        }
    }
}