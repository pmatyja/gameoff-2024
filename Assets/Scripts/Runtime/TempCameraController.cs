using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class TempCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] _cameras = new CinemachineCamera[4];
    
    [SerializeField] private KeyCode _camera1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode _camera2Key = KeyCode.Alpha2;
    [SerializeField] private KeyCode _camera3Key = KeyCode.Alpha3;
    [SerializeField] private KeyCode _camera4Key = KeyCode.Alpha4;

    void Start()
    {
        ValidateCameras();
        
        SetActiveCamera(0);
    }

    private void Update()
    {
        CollectInputs();
    }
    
    private void SetActiveCamera(int index)
    {
        for (var i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].gameObject.SetActive(i == index);
        }
    }
    
    private void CollectInputs()
    {
        if (Input.GetKeyDown(_camera1Key))
        {
            SetActiveCamera(0);
        }
        else if (Input.GetKeyDown(_camera2Key))
        {
            SetActiveCamera(1);
        }
        else if (Input.GetKeyDown(_camera3Key))
        {
            SetActiveCamera(2);
        }
        else if (Input.GetKeyDown(_camera4Key))
        {
            SetActiveCamera(3);
        }
    }

    private void ValidateCameras()
    {
        if (Enumerable.Any(_cameras, cam => !cam))
        {
            Debug.LogError("All 4 cameras must be assigned in the inspector.", this);
        }
    }
}
