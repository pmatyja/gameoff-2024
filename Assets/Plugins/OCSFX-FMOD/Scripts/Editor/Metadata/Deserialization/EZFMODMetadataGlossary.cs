namespace OCSFX.EZFMODEditor.Metadata.Deserialization
{
    public static class EZFMODMetadataGlossary
    {
        public struct Element
        {
            public const string OBJECTS = "objects";
            public const string OBJECT = "object";
            public const string PROPERTY = "property";
            public const string RELATIONSHIP = "relationship";
            public const string DESTINATION = "destination";
            public const string VALUE = "value";
        }
        
        public struct Attribute
        {
            public const string CLASS = "class";
            public const string ID = "id";
            public const string NAME = "name";
        }
        
        public struct Property
        {
            public const string NAME = "name";
            
            // Parameter
            public const string PARAMETER_TYPE = "parameterType";
            public const string IS_GLOBAL = "isGlobal";
            public const string INITIAL_VALUE = "initialValue";
            public const string IS_EXPOSED_RECURSIVELY = "isExposedRecursively";
            public const string MINIMUM = "minimum";
            public const string MAXIMUM = "maximum";
            public const string LABELS = "enumerationLabels";
            
            // Event
            public const string MIN_DISTANCE = "minimumDistance";
            public const string MAX_DISTANCE = "maximumDistance";
            public const string LENGTH = "length";
            public const string IS_3D = "is3D";
            
            // AudioFile
            public const string ASSET_PATH = "assetPath";
            
            // Bank
            public const string IS_MASTER_BANK = "isMasterBank";
        }
        
        public struct Relationship
        {
            public const string FOLDER = "folder";
            public const string BANKS = "banks";
            public const string PARAMETER = "parameter";
            public const string PRESET = "preset";
            public const string AUDIO_FILE = "audioFile";
            public const string MASTER_ASSET_FOLDER = "masterAssetFolder";
            
            // For Snapshot Groups
            public const string ITEMS = "items";
            
            public const string MARKER_TRACKS = "markerTracks";
        }
        
        public struct Class
        {
            public const string METADATA = "Metadata";
            public const string MASTER = "Master";
            public const string FOLDER = "Folder";
            
            public const string EVENT = "Event";
            public const string EVENT_FOLDER = EVENT + FOLDER;
            public const string MASTER_EVENT_FOLDER = MASTER + EVENT_FOLDER;
            
            public const string BANK = "Bank";
            public const string BANK_FOLDER = BANK + FOLDER;
            public const string MASTER_BANK_FOLDER = MASTER + BANK_FOLDER;
            
            public const string PARAMETER_PRESET = "ParameterPreset";
            public const string PARAMETER_PRESET_FOLDER = PARAMETER_PRESET + FOLDER;
            public const string MASTER_PARAMETER_PRESET_FOLDER = MASTER + PARAMETER_PRESET_FOLDER;
            public const string GAME_PARAMETER = "GameParameter";
            public const string PARAMETER_PROXY = "ParameterProxy";
            
            public const string SNAPSHOT = "Snapshot";
            public const string SNAPSHOT_GROUP = SNAPSHOT + "Group";
            public const string MASTER_SNAPSHOT_GROUP = SNAPSHOT + "List"; // SnapshotList... for some reason.
            
            public const string SINGLE_SOUND = "SingleSound";
            public const string MULTI_SOUND = "MultiSound";
            public const string AUDIO_FILE = "AudioFile";
            
            public const string SUSTAIN_POINT = "SustainPoint";
            public const string LOOP_REGION = "LoopRegion";
            public const string MAGNET_REGION = "MagnetRegion";
            
            public const string EFFECT_PRESET = "EffectPreset";
            public const string PROXY_EFFECT = "ProxyEffect";
            public const string SPATIALISER_EFFECT = "SpatialiserEffect";
            
            public const string TRANSITION_TIMELINE = "TransitionTimeline";
            public const string TRANSITION_REGION = "TransitionRegion";
            public const string TRANSITION_SOURCE_SOUND = "TransitionSourceSound";
            public const string TRANSITION_DESTINATION_SOUND = "TransitionDestinationSound";
            public const string MARKER_TRACK = "MarkerTrack";
        }

        public struct StudioPathRoot
        {
            public const string EVENT = "event:";
            public const string BANK = "bank:";
            public const string PARAMETER = "parameter:";
            public const string SNAPSHOT = "snapshot:";
        }
    }
}