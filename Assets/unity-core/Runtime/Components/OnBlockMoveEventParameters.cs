using FMODUnity;
using UnityEngine;

public struct OnBlockMoveEventParameters
{
    public GameObject BlockObject { get; private set; }
    public BlockDirection Direction { get; private set; }
    public Vector3 StartPosition { get; private set; }
    
    public OnBlockMoveEventParameters(GameObject blockObject, BlockDirection direction, Vector3 startPosition)
    {
        BlockObject = blockObject;
        Direction = direction;
        StartPosition = startPosition;
    }
}

public struct OnBlockMoveFinishedEventParameters
{
    public GameObject BlockObject { get; private set; }
    public BlockDirection Direction { get; private set; }
    public Vector3 EndPosition { get; private set; }
    
    public OnBlockMoveFinishedEventParameters(GameObject blockObject, BlockDirection direction, Vector3 endPosition)
    {
        BlockObject = blockObject;
        Direction = direction;
        EndPosition = endPosition;
    }
}