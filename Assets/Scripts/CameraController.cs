using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 8, -10); // offset from player
    public float rotationSpeed = 5f; // mouse rotation speed
    public float rotationStep = 90f;
    public float smoothRotationTime = 0.2f; // smooth transition time

    private float targetYaw; // target yaw (rotation around the y axis) angle for smooth transitions
    private float currentYaw; // current yaw angle

    void Start()
    {
        if (offset == Vector3.zero) // ensure default offset
        {
            offset = transform.position - player.position;
        }

        targetYaw = 0;
        currentYaw = targetYaw;
    }

    void LateUpdate()  //camera moves after the player
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) 
                targetYaw += rotationStep;
            if (Input.GetKeyDown(KeyCode.RightArrow)) 
                targetYaw -= rotationStep;
        }

        // the current yaw is smoothly going towards the target yaw
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime / smoothRotationTime);

        // update camera position and rotation
        Quaternion rotation = Quaternion.Euler(0, currentYaw, 0);
        transform.position = player.position + rotation * offset;
        transform.LookAt(player);
    }
}
