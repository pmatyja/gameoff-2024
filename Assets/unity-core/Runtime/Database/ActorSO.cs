using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ActorSO), menuName = "Lavgine/Database.Questing/Actor")]
[ScriptableObject("ACT")]
public class ActorSO : ResourceSO
{
    [SerializeField]
    [Translation]
    private string _name;
    public string Name => this._name;

    [SerializeField]
    [Translation]
    private string description;
    public string Description => this.description;

    [SerializeField]
    private Color color = Color.white;
    public Color Color => this.color;

    [SerializeField]
    private Sprite icon;
    public Sprite Icon => this.icon;

    [SerializeField]
    private Sprite avatar;
    public Sprite Avatar => this.avatar;

    [SerializeField]
    private List<ActorExpression> expressions = new();

    [SerializeField]
    private VoiceSO voice;
    public VoiceSO Voice => this.voice;
}

public abstract class ActorExpression
{
}

public class SpriteActorExpression : ActorExpression
{
    public Sprite Avatar;
}

public class AnimatedActorExpression : ActorExpression
{
    public SpriteAnimation Animation;
}

public class AnimationActorExpression : ActorExpression
{
    public AnimationClip Clip;
}