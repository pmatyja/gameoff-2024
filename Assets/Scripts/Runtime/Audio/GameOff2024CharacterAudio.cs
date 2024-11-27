using OCSFX.FMOD;
using Runtime.Audio;
using Runtime.Controllers;
using UnityEngine;

public class GameOff2024CharacterAudio : MonoBehaviour
{
    [SerializeField, Expandable] private GameOff2024CharacterAudioData _characterAudioData;
    [SerializeField] private GameOff2024Mover _mover;
    
    public void PlayFootstep()
    {
        // Don't play footstep if the character is in the air
        if (_mover && !_mover.IsGrounded) return;
        
        if (_characterAudioData && !_characterAudioData.Footstep.IsNull)
        {
            _characterAudioData.Footstep.Play(gameObject);
        }
    }
    
    public void PlayJump()
    {
        if (_characterAudioData && !_characterAudioData.Jump.IsNull)
        {
            _characterAudioData.Jump.Play(gameObject);
        }
    }
    
    public void PlayLand()
    {
        if (_characterAudioData && !_characterAudioData.Land.IsNull)
        {
            _characterAudioData.Land.Play(gameObject);
        }
    }
    
    public void PlayFoleyOneShot()
    {
        if (_characterAudioData && !_characterAudioData.FoleyOneShot.IsNull)
        {
            _characterAudioData.FoleyOneShot.Play(gameObject);
        }
    }
}
