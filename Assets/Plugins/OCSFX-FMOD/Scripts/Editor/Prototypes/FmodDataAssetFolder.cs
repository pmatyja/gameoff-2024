using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OCSFXEditor.FMOD.Prototype
{
    //[CreateAssetMenu(menuName = "OCSFX.FMOD/Editor/" + nameof(FmodDataAssetFolder))]
    public class FmodDataAssetFolder: ScriptableObject
    {
        [SerializeField] private List<ScriptableObject> _fmodDataObjects = new List<ScriptableObject>();

        public void Add(ScriptableObject subObject)
        {
            Debug.Log($"{this}: {nameof(Add)}", this);
            if (_fmodDataObjects.Contains(subObject)) return;
            _fmodDataObjects.Add(subObject);
            AssetDatabase.AddObjectToAsset(subObject, this);
        }
        
        public void Remove(ScriptableObject subObject)
        {
            Debug.Log($"{this}: {nameof(Remove)}", this);
            if (!_fmodDataObjects.Contains(subObject)) return;
            _fmodDataObjects.Remove(subObject);
            AssetDatabase.RemoveObjectFromAsset(subObject);
        }

        [ContextMenu("Test")]
        public void Test()
        {
            Debug.Log($"{this}: {nameof(Test)}", this);
        }
    }
}