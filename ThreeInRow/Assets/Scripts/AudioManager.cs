using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _matchSound;
    [SerializeField] private AudioClip _swapSound;
    [SerializeField] private AudioClip _failSound;

    public void PlayMatch()
    {
        if (_audioSource != null && _matchSound != null)
        {
            _audioSource.PlayOneShot(_matchSound);
        }
    }

    public void PlaySwap()
    {
        if (_audioSource != null && _swapSound != null)
        {
            _audioSource.PlayOneShot(_swapSound);
        }
    }

    public void PlayFail()
    {
        if (_audioSource != null && _failSound != null)
        {
            _audioSource.PlayOneShot(_failSound);
        }
    }
}
