using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionController : MonoBehaviour
{
    private LevelDataManager levelDataManager; // reference to the level data manager
    public Button[] levelButtons; // array of level buttons assigned in the inspector

    void OnEnable()
    {
        // find the level data manager in the current scene
        levelDataManager = FindObjectOfType<LevelDataManager>();

        // update the buttons to reflect the current level statuses
        updateLevelButtons();
    }

    public void LoadLevel(int levelNumber)
    {
        // load the scene corresponding to the selected level
        string sceneName = "Nivel" + levelNumber;
        SceneManager.LoadScene(sceneName);
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void updateLevelButtons()
    {
        // ensure level data is available
        if (levelDataManager.levelContainer == null || levelDataManager.levelContainer.levels == null)
        {
            return; // exit if no data is found
        }

        // update each button based on the level data
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < levelDataManager.levelContainer.levels.Count)
            {
                var levelData = levelDataManager.levelContainer.levels[i];
                levelButtons[i].interactable = (levelData.status == "unlocked"); // enable only unlocked levels
            }
        }
    }
}
