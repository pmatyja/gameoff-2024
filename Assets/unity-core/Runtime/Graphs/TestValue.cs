using UnityEngine;

public class TestValue : MonoBehaviour
{
    public bool TrueProperty => true;
    public bool FalseProperty => false;

    public readonly bool TrueField = true;

    public readonly bool FalseField = false;

    public bool GetTrue()
    {
        return true;
    }

    public bool GetFalse()
    {
        return false;
    }
}