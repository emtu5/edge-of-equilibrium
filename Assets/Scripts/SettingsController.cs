using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour
{
    public Toggle muteToggle;
    public Slider volumeSlider;

    void Start()
    {
        // Initialize the slider and toggle with current audio settings
        volumeSlider.value = AudioManager.Instance.GetVolume();  // Get current volume from AudioManager
        muteToggle.isOn = AudioManager.Instance.IsMuted();        // Get current mute state from AudioManager

        // Add listeners to the UI components
        muteToggle.onValueChanged.AddListener(MuteToggleChanged);
        volumeSlider.onValueChanged.AddListener(VolumeSliderChanged);
    }

    private void MuteToggleChanged(bool isMuted)
    {
        AudioManager.Instance.Mute(isMuted);  // Mute or unmute the audio via AudioManager
    }

    private void VolumeSliderChanged(float volume)
    {
        AudioManager.Instance.SetVolume(volume);  // Update the volume via AudioManager
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
