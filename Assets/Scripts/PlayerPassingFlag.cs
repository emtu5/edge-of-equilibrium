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

            //unlock the next level
            if (levelDataManager != null)
            {
                levelDataManager.UnlockNextLevel(currentLevel);
            }

            //load the next level scene
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
