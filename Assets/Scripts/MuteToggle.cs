using UnityEngine;
using UnityEngine.UI;

public class MuteToggle : MonoBehaviour
{
    public Toggle muteToggle;

    private void Start()
    {
        muteToggle.isOn = AudioManager.Instance.IsMuted();
        muteToggle.onValueChanged.AddListener(delegate { ToggleMute(); });
    }

    private void ToggleMute()
    {
        AudioManager.Instance.MuteToggle();
    }
}
