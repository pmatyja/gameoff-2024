using System;
using OCSFX.Attributes;
using OCSFX.Utility.Attributes;
using UnityEngine;

namespace Runtime.Utility
{
    public class GameOff2024UniqueID : MonoBehaviour
    {
        [field: SerializeField, ReadOnly] public string ID { get; private set; }

#if UNITY_EDITOR
        [SerializeField, Button(nameof(GenerateID))]
        private bool _generateIdButton;
#endif
        
        public void GenerateID() => ID = Guid.NewGuid().ToString();

        private void Reset() => GenerateID();
    }
}