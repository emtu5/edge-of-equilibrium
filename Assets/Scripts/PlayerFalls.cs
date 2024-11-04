using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFalls : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // if the player enters the trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player fell off the platform!");

            // restart the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
