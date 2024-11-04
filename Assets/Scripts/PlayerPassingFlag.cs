using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPassingFlag : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        // check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has reached the flag!");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
