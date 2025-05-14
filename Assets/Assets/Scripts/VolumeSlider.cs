using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        volumeSlider.value = AudioManager.Instance.GetVolume();
        volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });
    }

    private void ChangeVolume()
    {
        AudioManager.Instance.SetVolume(volumeSlider.value);
    }
}
