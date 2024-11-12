using System;
using UnityEngine;

[Serializable]
public class SpriteAnimator
{
    public Sprite Frame
    {
        get
        {
            if (this.animation == null)
            {
                return null;
            }

            if (this.animation.Frames.Length < 1)
            {
                return null;
            }

            return this.animation.Frames[this.frameIndex];
        }
    }

    [SerializeField]
    [Readonly]
    private float time;

    [SerializeField]
    [Readonly]
    private int frameIndex;

    private SpriteRenderer spriteRenderer;
    private SpriteAnimation[] animations;
    private SpriteAnimation animation;

    public SpriteAnimator(SpriteRenderer spriteRenderer, params SpriteAnimation[] animations)
    {
        if (spriteRenderer == null)
        {
            throw new ArgumentNullException("Sprite Renderer is null");
        }

        this.animations = animations;
        this.spriteRenderer = spriteRenderer;
        this.spriteRenderer.sprite = this.Frame;
    }

    public void Set(SpriteAnimation animation)
    {
        this.animation = animation;
        this.frameIndex = 0;
        this.time = 0.0f;

        if (this.animation != default)
        {
            this.spriteRenderer.sprite = this.Frame;
        }
    }

    public void Set(string id)
    {
        this.Set(this.animations.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase)));
    }

    public void Play(SpriteAnimation animation)
    {
        if (this.animation == animation)
        {
            return;
        }

        Debug.Log("SpriteAnimationSet");
        this.Set(animation);
    }

    public void Play(int hash)
    {
        this.Play(this.animations.FirstOrDefault(x => x.Hash == hash));
    }

    public void Play(string id)
    {
        this.Play(this.animations.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase)));
    }

    public void NextAnimation()
    {
        if (string.IsNullOrWhiteSpace(this.animation.NextAnimation))
        {
            return;
        }

        Debug.Log("NextAnimation");
        this.Play(this.animation.NextAnimation);
    }

    public void Update()
    {
        if (this.animation == null)
        {
            return;
        }

        this.time += Time.deltaTime;

        if (this.time > 1.0f / this.animation.FramePerSecond)
        {
            this.time = 0.0f;
            this.frameIndex++;

            if (this.frameIndex > this.animation.Frames.Length - 1)
            {
                this.frameIndex = 0;

                if (string.IsNullOrWhiteSpace(this.animation.NextAnimation) == false)
                {
                    this.Play(this.animation.NextAnimation);
                }
            }

            this.spriteRenderer.sprite = this.Frame;
        }
    }
}
