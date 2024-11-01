using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimationPlayer : MonoBehaviour
{
    [SerializeField]
    private SpriteAnimation[] animations;

    [SerializeField]
    private SpriteAnimator animator;

    public void Play(string id)
    {
        this.animator.Play(this.animations.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase)));
    }

    public void Play(SpriteAnimation spriteAnimation)
    {
        this.animator.Play(spriteAnimation);
    }

    private void Awake()
    {
        this.animator = new SpriteAnimator(this.GetComponent<SpriteRenderer>(), this.animations);
        this.animator.Play(this.animations.FirstOrDefault());
    }

    private void Update()
    {
        this.animator.Update();
    }
}
