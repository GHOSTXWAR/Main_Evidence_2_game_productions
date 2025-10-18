using UnityEngine;

public enum Ability { Fire }

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource movementSource;
    public AudioSource musicSource;
    public AudioSource Enemy;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float movementVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Ability Sounds")]
    public AudioClip fireSound;

    [Header("Movement Sounds")]
    public AudioClip footstepSound;
    public AudioClip jumpSound;

    [Header("Enemy Sounds")]
    public AudioClip EnemySound;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        LoadVolumeSettings();
    }

    void Start()
    {
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        if (movementSource != null)
        {
            movementSource.volume = movementVolume;
        }

        if (sfxSource != null)
            {
            sfxSource.volume = sfxVolume;
            }
    }

    void Update()
    {

    }

    public void PlayAbilitySound(Ability ability)
    {
        AudioClip clip = null;

        switch (ability)
        {
            case Ability.Fire: clip = fireSound; break;  
        }

        if (clip)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayFootstepSound()
    {
        if (footstepSound && !movementSource.isPlaying)
        {
            movementSource.clip = footstepSound;
            movementSource.volume = movementVolume;
            movementSource.Play();
        }
    }

    public void StopFootstepSound()
    {
        if (movementSource.isPlaying)
        {
            movementSource.Stop();
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound)
            sfxSource.PlayOneShot(jumpSound, sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
            musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void SetMovementVolume(float volume)
    {
        movementVolume = volume;
        if (movementSource != null)
            movementSource.volume = movementVolume;
        PlayerPrefs.SetFloat("MovementVolume", movementVolume);
    }

    public void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        movementVolume = PlayerPrefs.GetFloat("MovementVolume", 1f);

        if (musicSource != null)
            musicSource.volume = musicVolume;

        if (movementSource != null)
            movementSource.volume = movementVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    public void PlayEnemySound()
    {
        if (EnemySound && !Enemy.isPlaying)
        {
           Enemy.clip =EnemySound;
           Enemy.volume = sfxVolume;
           Enemy.Play();
        }
    }

    public void StopEnemySound()
    {
        if (EnemySound != null && Enemy.isPlaying)
        {
            Enemy.Stop();
        }
    }
    private void OnDisable()
    {
        StopFootstepSound();
        if (EnemySound != null) StopEnemySound();   
    }
    
}
