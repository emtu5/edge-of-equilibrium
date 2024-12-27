using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeSystem : MonoBehaviour
{
    public int maxLives = 3; // maximum number of lives
    public int lives; // current number of lives
    public GameObject[] hearts; // array of heart UI icons
    private CheckpointSystem checkpointSystem;
    private bool isRespawning = false; // flag to prevent multiple triggers

    private static LifeSystem instance; // singleton instance

    void Awake()
    {
        // singleton pattern to persist LifeSystem across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // make this object persistent
        }
        else if (instance != this)
        {
            Destroy(gameObject); // destroy duplicate instances
        }
    }

    void Start()
    {
        // initialize lives and UI
        lives = maxLives;
        UpdateHeartUI();

        // find the checkpoint system in the scene
        checkpointSystem = FindObjectOfType<CheckpointSystem>();
    }

    public void LoseLife()
    {
        // prevent multiple triggers during respawn
        if (isRespawning) return;

        isRespawning = true; // set flag to prevent re-triggering
        lives--; // decrement lives
        UpdateHeartUI(); // update UI to reflect lives left

        if (lives > 0)
        {
            // respawn player at last checkpoint
            if (checkpointSystem != null)
            {
                checkpointSystem.RespawnPlayer();
            }
        }
        else
        {
            // restart level if no lives left
            RestartLevel();
        }

        // reset the respawning flag after a short delay
        Invoke(nameof(ResetRespawningFlag), 0.5f);
    }

    public void UpdateHeartUI()
    {
        // show hearts for remaining lives and hide the rest
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < lives);
        }
    }

    private void RestartLevel()
    {
        // reset lives and checkpoint data
        lives = maxLives; // reset lives to maximum
        UpdateHeartUI(); // immediately update the heart UI to show full hearts
        PlayerPrefs.DeleteAll(); // clear saved checkpoints
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload the level
    }

    private void ResetRespawningFlag()
    {
        isRespawning = false; // allow LoseLife to be triggered again
    }
}
