using OCSFX.Utility.Debug;
using Runtime;
using UnityEngine;

public class ExitDoor : InteractableDoor
{
    [Space]
    [SerializeField] private BlockMoverScript _blockMoverScript;

    private void Awake()
    {
        if (!_blockMoverScript) _blockMoverScript = GetComponent<BlockMoverScript>();
        
        if (!_blockMoverScript)
        {
            OCSFXLogger.LogError($"{nameof(BlockMoverScript)} not found on {nameof(ExitDoor)} GameObject ({name})", this, _showDebug);
        }
        
        Lock();
    }

    protected override void Open()
    {
        base.Open();
        
        _blockMoverScript.Activate();
    }
}
