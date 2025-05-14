using UnityEngine;

public class MaterialController : MonoBehaviour
{
    public BallMaterial ballMaterial;
    private PlayerMovement playerMovement;

    private Rigidbody rb;
    private Renderer renderer;

    void Start()
    {
        // Get the Rigidbody and Renderer components
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        playerMovement = GetComponent<PlayerMovement>();

        // Apply the BallType properties
        ApplyBallMaterial();
    }

    void ApplyBallMaterial()
    {
        if (ballMaterial != null)
        {
            Debug.Log($"Applying BallMaterial: {ballMaterial.name}");

            if (renderer == null)
            {
                Debug.LogError("Renderer is null. Ensure this GameObject has a Renderer component.");
            }

            if (rb == null)
            {
                Debug.LogError("Rigidbody is null. Ensure this GameObject has a Rigidbody component.");
            }

            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement is null. Ensure the PlayerMovement component is attached.");
            }

            // Set the material
            renderer.material = ballMaterial.material;

            // Set the physic material
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.material = ballMaterial.physicMaterial;
            }

            // Set the mass
            rb.mass = ballMaterial.mass;
			rb.drag = ballMaterial.drag;
            playerMovement.setSpeed(ballMaterial.speed);
        }
    }

    public void SetBallMaterial(BallMaterial newMaterial)
    {
        ballMaterial = newMaterial;
        ApplyBallMaterial();
    }

}
