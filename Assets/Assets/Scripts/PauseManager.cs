using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;  // Reference to the Pause Panel
    private bool isPaused = false; // Track whether the game is paused

    // Toggle pause state
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Pause the game
    public void PauseGame()
    {
        Time.timeScale = 0f;         // Freeze time
        isPaused = true;             // Set pause state
        pausePanel.SetActive(true);  // Show pause menu
    }

    // Resume the game
    public void ResumeGame()
    {
        Time.timeScale = 1f;         // Resume time
        isPaused = false;            // Reset pause state
        pausePanel.SetActive(false); // Hide pause menu
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player object
        player.GetComponent<CheckpointSystem>().RespawnPlayer(); // Call the Respawn method on the player
        ResumeGame();
    }
}
