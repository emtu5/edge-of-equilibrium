using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFalls : MonoBehaviour
{
    public CheckpointSystem checkpointSystem;
    private void OnTriggerEnter(Collider other)
    {
        // if the player enters the trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player fell off the platform!");

            // respawn the player using the checkpoint system script
            checkpointSystem.RespawnPlayer();
        }
    }
}
