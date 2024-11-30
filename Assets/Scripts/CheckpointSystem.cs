using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 respawnPosition; 
    public Transform player; 
    void Start()
    {
        respawnPosition = player.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            //update player respawn position
            respawnPosition = other.transform.position;
            Debug.Log("Checkpoint reached!");
        }
    }
    public void RespawnPlayer()
    {
        player.position = respawnPosition;
    }
}
