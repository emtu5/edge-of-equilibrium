using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;

    public AudioClip dreamyTheme;
    public AudioClip darkTheme;

    private bool isMuted = false;
    private float volume = 1.0f;

    private void Awake()
    {
        // Singleton pattern to ensure only one AudioManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(dreamyTheme);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void MuteToggle()
    {
        isMuted = !isMuted;
        musicSource.mute = isMuted;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        musicSource.volume = volume;
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public float GetVolume()
    {
        return volume;
    }
}
