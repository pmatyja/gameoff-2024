using System;

public class NodeGraphContext : INodeGraphContext
{
    public Guid Id { get; } = Guid.NewGuid();
}