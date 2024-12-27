using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPassingFlag : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private int currentLevel; 

    private LevelDataManager levelDataManager;

    private void Start()
    {
        // find the LevelDataManager in the scene
        levelDataManager = FindObjectOfType<LevelDataManager>();
        if (levelDataManager == null)
        {
            Debug.LogError("LevelDataManager not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has reached the flag!");

            //remove all player prefs (respawn info)
            PlayerPrefs.DeleteAll();

            //unlock the next level
            if (levelDataManager != null)
            {
                levelDataManager.UnlockNextLevel(currentLevel);
            }

            // reset lives for the next level
            LifeSystem lifeSystem = FindObjectOfType<LifeSystem>();
            if (lifeSystem != null)
            {
                lifeSystem.lives = 3; // reset lives to 3
                lifeSystem.UpdateHeartUI(); // ensure hearts are updated
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}
