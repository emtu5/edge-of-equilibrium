using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip dreamyTheme;
    public AudioClip darkTheme;

    private AudioSource audioSource;  // AudioSource component for playing the music

    // Default volume and mute settings
    private float volume = 0.5f;
    private bool isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy the duplicate instance of the AudioManager
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Loop the music by default
        audioSource.volume = volume; // Set the starting volume level
    }

    public void PlayDreamyTheme()
    {
        audioSource.clip = dreamyTheme;
        audioSource.Play();
    }

    public void PlayDarkTheme()
    {
        audioSource.clip = darkTheme;
        audioSource.Play();
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        audioSource.volume = volume;
    }

    public void Mute(bool mute)
    {
        isMuted = mute;
        audioSource.mute = isMuted;
    }

    public float GetVolume()
    {
        return volume;
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}
