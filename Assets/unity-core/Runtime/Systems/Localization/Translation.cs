using System;
using UnityEngine;

[Serializable]
public class Translation
{
    [SerializeField]
    [HideLabel]
    private string name;
    public string Name => this.name;

    [SerializeField]
    [HideLabel]
    [Multiline(5)]
    private string value;
    public string Value => value;

    public void Normalize()
    {
        this.name = this.name.ToLower();
    }
}