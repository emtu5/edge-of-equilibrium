using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private LevelDataManager levelDataManager; // reference to the level data manager

    void Start()
    {
        initializeLevelDataManager(); // find or assign the level data manager when the menu loads
    }

    public void StartNewGame()
    {
        if (levelDataManager != null)
        {
            LifeSystem.lives = 3;
            LifeSystem.collectedHearts.Clear();
            //resetLevelProgress(); 
            SceneManager.LoadScene("Nivel1");
        }
    }

    public void LoadGame()
    {
        LifeSystem.lives = 3;
        LifeSystem.collectedHearts.Clear();
        SceneManager.LoadScene("LevelSelection"); 
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void GoToInfoScene()
    {
        SceneManager.LoadScene("InfoScene");
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); 
    }

    private void resetLevelProgress()
    {
        // ensure the level container is initialized
        if (levelDataManager.levelContainer == null)
        {
            levelDataManager.levelContainer = new LevelDataManager.LevelContainer();
        }

        levelDataManager.levelContainer.levels.Clear(); // clear existing levels

        // create new levels with only the first one unlocked
        for (int i = 1; i <= 3; i++) 
        {
            levelDataManager.levelContainer.levels.Add(new LevelDataManager.LevelData
            {
                level = i,
                status = (i == 1) ? "unlocked" : "locked"
            });
        }

        levelDataManager.SaveLevelData(); // save the updated data
    }

    private void initializeLevelDataManager()
    {
        // find the existing level data manager in the scene
        if (levelDataManager == null)
        {
            levelDataManager = FindObjectOfType<LevelDataManager>();
        }
    }
}
