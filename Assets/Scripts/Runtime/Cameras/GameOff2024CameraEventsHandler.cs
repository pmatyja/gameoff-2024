using System;
using Runtime;
using Runtime.Cameras;
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
    
    public static event Action<bool> OnGameOff2024CameraStatusChangedEvent;

    public static bool IsGameOff2024CameraActive { get; private set; }

    private GameOff2024CameraControllerBase _gameOff2024Camera;

    private void OnEnable()
    {
        _gameOff2024Camera = GameOff2024Statics.GetGameOff2024Camera();
        
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
        Debug.Log($"{nameof(OnCameraActivated)}: {cinemachineCamera.Name}", this);
        
        UpdateGameOffCameraStatus(cinemachineCamera);
        
        OnCameraActivatedEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }

    private void OnCameraDeactivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
    {
        Debug.Log($"{nameof(OnCameraDeactivated)}: {cinemachineCamera.Name}", this);
        
        OnCameraDeactivatedEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }
    
    private void OnCameraCut(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
    {
        Debug.Log($"{nameof(OnCameraCut)}: {cinemachineCamera.Name}", this);
        
        OnCameraCutEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }
    
    private void OnBlendCreated(CinemachineCore.BlendEventParams blendEventParams)
    {
        var startCamera = blendEventParams.Blend.CamA;
        var destinationCamera = blendEventParams.Blend.CamB;
        UpdateGameOffCameraStatus(destinationCamera);
        
        Debug.Log($"{nameof(OnBlendCreated)}: From [{startCamera.Name}] To [{destinationCamera.Name}]", this);
        
        OnBlendCreatedEvent?.Invoke(blendEventParams);
    }
    
    private void OnBlendFinished(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
    {
        UpdateGameOffCameraStatus(cinemachineCamera);
        
        Debug.Log($"{nameof(OnBlendFinished)}: {cinemachineCamera.Name}", this);
        
        OnBlendFinishedEvent?.Invoke(cinemachineMixer, cinemachineCamera);
    }
    
    private void UpdateGameOffCameraStatus(ICinemachineCamera cinemachineCameraComparison)
    {
        if (!_gameOff2024Camera) _gameOff2024Camera = GameOff2024Statics.GetGameOff2024Camera();
        
        if (cinemachineCameraComparison is CinemachineCamera cinemachineCamera)
        {
            IsGameOff2024CameraActive =
                cinemachineCamera == _gameOff2024Camera.GetCinemachineCamera()
                && cinemachineCamera == (CinemachineCamera)_cinemachineBrainEvents.Brain.ActiveVirtualCamera;
        }
        
        OnGameOff2024CameraStatusChangedEvent?.Invoke(IsGameOff2024CameraActive);
        
        Debug.Log($"{nameof(UpdateGameOffCameraStatus)}: {IsGameOff2024CameraActive}", this);
    }
}
