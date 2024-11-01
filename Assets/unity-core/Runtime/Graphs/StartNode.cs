using System;
using System.Collections;
using UnityEngine;

[Serializable]
[DisallowMultiple]
public class StartNode : INode, IFlowNode
{
    public float Width => 192.0f;
    public string BackgroundColor { get; } = "#8C4134";

    public bool IsCutscene;

    [Output]
    public Node Out;

    public IEnumerator ExcuteAsync(INodeGraphContext context)
    {
        if (this.IsCutscene)
        {
            GameManager.Mode = GameMode.Cutscene;
        }

        if (this.Out == null)
        {
            Logger.Warning("StarNode has not connections");
        }

        yield return this.Out?.ExcuteAsync(context);

        if (this.IsCutscene)
        {
            GameManager.Mode = GameMode.Gameplay;
        }
    }
}
