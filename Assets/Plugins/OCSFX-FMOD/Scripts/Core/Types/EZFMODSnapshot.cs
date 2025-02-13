using FMOD;

namespace OCSFX.EZFMOD.Types
{
    public class EZFMODSnapshot : EZFMODEventBase
    {
        internal void Init(string inName, string path, GUID guid)
        {
            Name = inName;
            StudioPath = path;
            GUID = guid;
        }
    }
}