using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Sources ----------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource movementSource;

    [Header("---------- Audio Clips ----------")]
    public AudioClip background;
    public AudioClip coinCollector;
    public AudioClip playerHurtEnemy;
    public AudioClip playerHurtTrap;
    public AudioClip walkStep;
    public AudioClip jumpSound;  // New jump sound effect

    private void Start()
    {
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.Play();
        }

        // Load saved volume from PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f); // Default to 1 if no saved volume
        SetMusicVolume(savedVolume);
    }

    // Set the music volume
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume); // Save the volume setting
        }
    }

    // New method to access musicSource for VolumeControl
    public AudioSource GetMusicSource()
    {
        return musicSource;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            SFXSource.PlayOneShot(clip);
    }

    public void PlayMovementSound()
    {
        if (!movementSource.isPlaying && walkStep != null)
        {
            movementSource.clip = walkStep;
            movementSource.loop = true;
            movementSource.Play();
        }
    }

    public void StopMovementSound()
    {
        if (movementSource.isPlaying)
        {
            movementSource.Stop();
        }
    }

    public void PlayHurtByEnemy()
    {
        PlaySFX(playerHurtEnemy);
    }

    public void PlayHurtByTrap()
    {
        PlaySFX(playerHurtTrap);
    }

    public void PlayCoinCollect()
    {
        PlaySFX(coinCollector);
    }

    // New method to play the jump sound
    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            SFXSource.PlayOneShot(jumpSound);
        }
    }
}
