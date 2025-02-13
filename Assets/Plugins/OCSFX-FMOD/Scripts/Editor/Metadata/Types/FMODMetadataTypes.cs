using OCSFX.EZFMOD.Types;
using GUID = FMOD.GUID;

namespace OCSFX.EZFMODEditor.Metadata.Types
{
    public interface IEZFMODComparable<in T> where T : EZFMODAsset
    {
        public bool HasSameDataAs(T ezFmodAsset);
        public bool InitializeIfDifferent(T ezFmodAsset);
    }

    public interface IFMODFolder
    {
        public GUID FolderGUID { get; }

        public string GetFolderRelationship();
    }

    public interface IFMODDuration
    {
        public double Length { get; }
        public string GetLengthProperty();
    }
}