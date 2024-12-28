using System.Collections.Generic;
using UnityEngine;


public class LifeSystem : MonoBehaviour
{
    public GameObject heartPrefab; // prefab for heart UI
    public Transform heartsParent; // parent object for hearts in the UI
    public int maxLives = 5;
    public int startLives = 3;

    public static int lives; // static variable to persist lives
    //track hearts that have been collected so that they don't respawn at every fall
    public static HashSet<string> collectedHearts = new HashSet<string>(); 

    private void Start()
    {
        // if lives hasn't been set, initialize it to startLives
        if (lives == 0)
        {
            lives = startLives;
        }

        UpdateHeartUI();
    }

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--;
            UpdateHeartUI();
        }

        if (lives <= 0)
        {
            collectedHearts.Clear(); 
            lives = startLives;
            UpdateHeartUI(); 
           
        }
    }

    public void AddLife()
    {
        if (lives < maxLives)
        {
            lives++;
            UpdateHeartUI(); 
        }
    }

    public void UpdateHeartUI()
    {
        // clear existing hearts
        foreach (Transform child in heartsParent)
        {
            Destroy(child.gameObject);
        }

        // add hearts based on the current number of lives
        for (int i = 0; i < lives; i++)
        {
            Instantiate(heartPrefab, heartsParent);
        }
    }
}
