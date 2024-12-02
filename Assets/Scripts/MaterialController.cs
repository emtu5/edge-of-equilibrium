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
            playerMovement.setSpeed(ballMaterial.speed);
        }
    }

    public void SetBallMaterial(BallMaterial newMaterial)
    {
        ballMaterial = newMaterial;
        ApplyBallMaterial();
    }

}
