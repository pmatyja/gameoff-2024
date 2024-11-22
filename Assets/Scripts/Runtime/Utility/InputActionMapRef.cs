using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Utility
{
    [Serializable]
    public class InputActionMapRef
    {
        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField] private string _name;
        
        [SerializeField] private InputActionMap _map;
        public InputActionMap Map => _map;
        
        public InputActionAsset InputActionAsset => _inputActionAsset;
        public string Name => _name;
        
        public InputActionMapRef(InputActionAsset inputActionAsset, string name)
        {
            _inputActionAsset = inputActionAsset;
            _name = name;
            
            _map = _inputActionAsset.FindActionMap(_name);
        }
        
        public void UpdateMap()
        {
            _map = _inputActionAsset.FindActionMap(_name);
        }
    }
}