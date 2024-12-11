using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // Start the game
    public void StartGame()
    {
        SceneManager.LoadScene("Nivel1");
    }

    // Open the settings
    //public void OpenSettings()
    //{
    //    SceneManager.LoadScene("SettingsScene");
    //}

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}