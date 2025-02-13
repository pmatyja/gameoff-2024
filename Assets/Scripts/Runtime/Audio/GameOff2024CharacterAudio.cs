using OCSFX.EZFMOD;
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
        
        _characterAudioData?.Footstep?.Play(gameObject);
    }
    
    public void PlayJump()
    {
        _characterAudioData?.Jump?.Play(gameObject);
    }
    
    public void PlayLand()
    {
        _characterAudioData?.Land?.Play(gameObject);
    }
    
    public void PlayFoleyOneShot()
    {
        _characterAudioData?.FoleyOneShot?.Play(gameObject);
    }
}
