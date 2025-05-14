using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (AudioManager.Instance == null) return;

        // Switch music based on the scene name
        switch (scene.name)
        {
            case "MainMenu":
                AudioManager.Instance.PlayMusic(AudioManager.Instance.dreamyTheme);
                break;

            case "Nivel1":
            case "Nivel2":
                AudioManager.Instance.PlayMusic(AudioManager.Instance.dreamyTheme);
                //AudioManager.Instance.musicSource.time = 0f;
                break;

            case "Nivel3":
            case "Nivel4":
                AudioManager.Instance.PlayMusic(AudioManager.Instance.darkTheme);
                break;
        }
    }
}
