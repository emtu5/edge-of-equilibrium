using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelDataManager : MonoBehaviour
{
    private static LevelDataManager instance; // singleton instance to ensure only one manager exists
    private string saveFilePath; // path to save and load the level data
    public LevelContainer levelContainer; // container to hold all level data

    void Awake()
    {
        if (instance == null)
        {
            // initialize singleton instance and persist across scenes
            instance = this;
            DontDestroyOnLoad(gameObject);

            // set the save file path for the level data
            saveFilePath = Path.Combine(Application.persistentDataPath, "levelData.json");
            Debug.Log(saveFilePath);
            // load the level data when the game starts
            LoadLevelData();
        }
        else
        {
            // destroy any duplicate instances to enforce singleton behavior
            Destroy(gameObject);
        }
    }

    public void SaveLevelData()
    {
        // convert level data to json format and save it to the file
        string json = JsonUtility.ToJson(levelContainer, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void UnlockNextLevel(int currentLevel)
    {
        // unlock the next level by updating its status
        foreach (var level in levelContainer.levels)
        {
            if (level.level == currentLevel + 1)
            {
                level.status = "unlocked"; 
                SaveLevelData(); // save the updated data
                break;
            }
        }
    }

    public void LoadLevelData()
    {
        if (levelContainer == null)
        {
            levelContainer = new LevelContainer();
        }

        // load level data from the save file if it exists
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            levelContainer = JsonUtility.FromJson<LevelContainer>(json);

            // if the loaded data is invalid, create default level data
            if (levelContainer == null || levelContainer.levels == null || levelContainer.levels.Count == 0)
            {
                CreateDefaultLevelData();
            }
        }
        else
        {
            // create default data if no save file exists
            CreateDefaultLevelData();
        }
    }

    private void CreateDefaultLevelData()
    {
        // initialize default level data (first level unlocked, others locked)
        levelContainer = new LevelContainer();

        for (int i = 1; i <= 3; i++) 
        {
            levelContainer.levels.Add(new LevelData
            {
                level = i,
                status = (i == 1) ? "unlocked" : "locked" 
            });
        }

        // save the default level data to a file
        SaveLevelData();
    }

    [System.Serializable]
    public class LevelData
    {
        public int level; // the level number
        public string status; // status of the level (unlocked or locked)
    }

    [System.Serializable]
    public class LevelContainer
    {
        public List<LevelData> levels = new List<LevelData>(); // list of all levels
    }
}
