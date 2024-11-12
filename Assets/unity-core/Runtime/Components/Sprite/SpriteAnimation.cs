using System;
using UnityEngine;

[Serializable]
public class SpriteAnimation
{
    public int Hash
    {
        get
        {
            var result = Animator.StringToHash(this.id);
            return result;
        }
    }

    [SerializeField]
    private string id;
    public string Id => this.id;

    [SerializeField]
    [Range(1, 60)]
    private float framePerSecond = 24;
    public float FramePerSecond => this.framePerSecond;

    [SerializeField]
    private string nextAnimation;
    public string NextAnimation => this.nextAnimation;

    [SerializeField]
    private Sprite[] frames;
    public Sprite[] Frames => this.frames;
}
