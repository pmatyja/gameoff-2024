using System;

[Serializable]
public class NodeConnection
{
    [Readonly]
    public bool IsValueNode;

    [Readonly]
    public string OutputNode;

    [Readonly]
    public string OutputPort;

    [Readonly]
    public string OutputPortName;

    [Readonly]
    public string InputNode;

    [Readonly]
    public string InputPort;

    [Readonly]
    public string InputPortName;
}
