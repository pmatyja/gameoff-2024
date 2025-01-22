using OCSFX.FMOD.Types;
using OCSFX.Utility;
using FMODUnity;
using OCSFX.FMOD.Prototype;
using UnityEngine;
using Debug = UnityEngine.Debug;
using ParameterType = OCSFX.FMOD.Prototype.ParameterType;

#if UNITY_EDITOR
using FMOD.Studio;
using UnityEditor;
#endif //UNITY_EDITOR

namespace OCSFX.FMOD
{
    [CreateAssetMenu(menuName = "OCSFX/Prototype/FmodParameterObject")]
    public class FmodParameterObject : ScriptableObject
    {
        [SerializeField] private FMODParameter _fmodParameter;
        [SerializeField] private FmodParameterObjectData _data;

        public void SetGlobalValue(float newValue)
        {
            if (string.IsNullOrWhiteSpace(_fmodParameter.Parameter)) return;

            _fmodParameter.Value = newValue;
            
            // Debug.Log($"Set Param ({_fmodParameter.Parameter}) to {_fmodParameter.Value}");
            OCSFXAudioStatics.SetFMODParameterGlobal(_fmodParameter.Parameter, _fmodParameter.Value);
        }

        public void SetGlobalValue(int newValue)
        {
            if (string.IsNullOrWhiteSpace(_fmodParameter.Parameter)) return;

            _fmodParameter.Value = newValue;
            
            OCSFXAudioStatics.SetFMODParameterGlobal(_fmodParameter.Parameter, _fmodParameter.Value);
        }

        public float GetGlobalValue()
        {
            return OCSFXAudioStatics.GetFMODParameterGlobalValue(_fmodParameter.Parameter);
        }

        public void SetValue(float value, GameObject gameObject)
        {
            var fmodGameObject = gameObject.GetOrAdd<FMODGameObject>();
            
            fmodGameObject.SetParameter(_fmodParameter.Parameter, value);
        }

        public float GetValue(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<FMODGameObject>(out var fmodGameObject))
                fmodGameObject = gameObject.AddComponent<FMODGameObject>();

            return fmodGameObject.GetParameterValue(_fmodParameter.Parameter);
        }
        
        private void Initialize()
        {
            _fmodParameter.Parameter = name;
            
#if UNITY_EDITOR
            var editorParamRef = EventManager.Parameters.Find(editorParamRef => editorParamRef.Name == _fmodParameter.Parameter);

            if (!editorParamRef)
            {
                Debug.LogWarning($"No parameter reference found for {_fmodParameter.Parameter}", this);

                _data = default;
                return;
            }
            
            // EditorUtils.System.getParameterDescriptionByName(_fmodParameter.Parameter, out var paramDesc);
            // var type = paramDesc.type;

            name = editorParamRef.Name;

            if (TryUpdateData(editorParamRef))
            {
                OnDataChanged();
            }
#endif //UNITY_EDITOR
        }

#if UNITY_EDITOR
        private bool TryUpdateData(EditorParamRef editorParamRef)
        {
            bool changed;
            
            if (!_data.IsNull())
            {
                changed = _data.SetData(
                    editorParamRef.ID,
                    editorParamRef.IsGlobal,
                    editorParamRef.Labels,
                    editorParamRef.Min,
                    editorParamRef.Max,
                    editorParamRef.Default,
                    (ParameterType)editorParamRef.Type,
                    editorParamRef.Exists);
            }
            else
            {
                _data = new FmodParameterObjectData(
                    editorParamRef.ID,
                    editorParamRef.IsGlobal,
                    editorParamRef.Labels,
                    editorParamRef.Min,
                    editorParamRef.Max,
                    editorParamRef.Default,
                    (ParameterType)editorParamRef.Type,
                    editorParamRef.Exists
                );

                changed = true;
            }

            return changed;
        }
    #endif //UNITY_EDITOR

        private void OnDataChanged()
        {
            Debug.Log($"{this} Initialized", this);
            SetGlobalValue(_data.DefaultValue);
        }

        private void OnEnable()
        {
            Initialize();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                SetGlobalValue(_fmodParameter.Value);
                Application.quitting += OnApplicationQuitting;
            }
#endif //UNITY_EDITOR
        }
        
#if UNITY_EDITOR
        private void OnApplicationQuitting()
        {
            Application.quitting -= OnApplicationQuitting;
            SetGlobalValue(_data.DefaultValue);
        }
#endif //UNITY_EDITOR

        private void OnValidate()
        {
            Initialize();
#if UNITY_EDITOR
            if (Application.isPlaying)
                SetGlobalValue(_fmodParameter.Value);
#endif //UNITY_EDITOR
            _fmodParameter.Value = Mathf.Clamp(_fmodParameter.Value, _data.MinValue, _data.MaxValue);
        }

        private void Reset()
        {
            Initialize();
#if UNITY_EDITOR
            if (Application.isPlaying)
                SetGlobalValue(_fmodParameter.Value);
#endif //UNITY_EDITOR
        }
    }
}
