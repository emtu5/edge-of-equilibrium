using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 respawnPosition;
    private Rigidbody rigidbody;
    public Transform player; 
    void Start()
    {
        respawnPosition = player.position;
        rigidbody = player.GetComponent<Rigidbody>();
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

        // reset player momentum
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.rotation = Quaternion.identity;
    }
}
