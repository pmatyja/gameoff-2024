using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimationPlayer : MonoBehaviour
{
    [SerializeField]
    [Readonly]
    private Animator controller;

    [SerializeField]
    private SpriteAnimation[] animations;

    private SpriteAnimator animator;

    private AnimatorClipInfo[] clipInfo;

    public void Play(string id)
    {
        this.animator.Play(this.animations.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase)));
    }

    public void Play(SpriteAnimation animation)
    {
        this.animator.Play(animation);
    }

    public void NextAnimation()
    {
        this.animator.NextAnimation();
    }

    private void Awake()
    {
        this.animator = new SpriteAnimator(this.GetComponent<SpriteRenderer>(), this.animations);
        this.animator.Play(this.animations.FirstOrDefault());

        this.TryGetComponent<Animator>(out this.controller);
    }

    private void Update()
    {
        this.animator.Update();

        if (this.controller)
        {
            var state = this.controller.GetCurrentAnimatorStateInfo(0);

            this.animator.Play(state.tagHash);
        }
    }
}
