using System;
using UnityEngine;

[Serializable]
public class DialogEventParameters
{
    [Readonly]
    [Range(0, 255)]
    public byte Priority;

    [Readonly]
    [TextArea(3, 3)]
    public string Content;

    [Readonly]
    public VoiceLineSO VoiceLine;
}
