using OCSFX.FMOD;
using Runtime.Audio;
using UnityEngine;

public class GameOff2024CharacterAudio : MonoBehaviour
{
    [SerializeField, Expandable] private GameOff2024CharacterAudioData _characterAudioData;
    
    public void PlayFootstep()
    {
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
