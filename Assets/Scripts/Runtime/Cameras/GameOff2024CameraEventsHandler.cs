using System;
using Unity.Cinemachine;
using UnityEngine;

public class GameOff2024CameraEventsHandler : MonoBehaviour
{
    [SerializeField] private CinemachineBrainEvents _cinemachineBrainEvents;
    
    public static event Action<ICinemachineMixer, ICinemachineCamera> OnCameraActivatedEvent;
    public static event Action<ICinemachineMixer, ICinemachineCamera> OnCameraDeactivatedEvent;
    
    public static event Action<ICinemachineMixer, ICinemachineCamera> OnCameraCutEvent;
    
    public static event Action<CinemachineCore.BlendEventParams> OnBlendCreatedEvent;
    public static event Action<ICinemachineMixer, ICinemachineCamera> OnBlendFinishedEvent;
    
    private void OnEnable()
    {
        if (!_cinemachineBrainEvents)
        {
            if (!TryGetComponent(out _cinemachineBrainEvents))
            {
                _cinemachineBrainEvents = FindAnyObjectByType<CinemachineBrainEvents>();   
                
                if (!_cinemachineBrainEvents)
                {
                    Debug.LogError($"No instance of {nameof(CinemachineBrainEvents)} found in the scene.");
                    return;
                }
            }
        }
        
        _cinemachineBrainEvents.CameraActivatedEvent.AddListener(OnCameraActivated);
        _cinemachineBrainEvents.CameraDeactivatedEvent.AddListener(OnCameraDeactivated);
        
        _cinemachineBrainEvents.CameraCutEvent.AddListener(OnCameraCut);
        
        _cinemachineBrainEvents.BlendCreatedEvent.AddListener(OnBlendCreated);
        _cinemachineBrainEvents.BlendFinishedEvent.AddListener(OnBlendFinished);
    }
    
    private void OnDisable()
    {
        if (!_cinemachineBrainEvents) _cinemachineBrainEvents = FindAnyObjectByType<CinemachineBrainEvents>();
        if (!_cinemachineBrainEvents) return;
        
        _cinemachineBrainEvents.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        _cinemachineBrainEvents.CameraDeactivatedEvent.RemoveListener(OnCameraDeactivated);
        
        _cinemachineBrainEvents.CameraCutEvent.RemoveListener(OnCameraCut);
        
        _cinemachineBrainEvents.BlendCreatedEvent.RemoveListener(OnBlendCreated);
        _cinemachineBrainEvents.BlendFinishedEvent.RemoveListener(OnBlendFinished);
    }

    private void OnCameraActivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
    {
        OnCameraActivatedEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }
    
    private void OnCameraDeactivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
    {
        OnCameraDeactivatedEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }
    
    private void OnCameraCut(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
    {
        OnCameraCutEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }
    
    private void OnBlendCreated(CinemachineCore.BlendEventParams blendEventParams)
    {
        OnBlendCreatedEvent?.Invoke(blendEventParams);
    }
    
    private void OnBlendFinished(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
    {
        OnBlendFinishedEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }
}
