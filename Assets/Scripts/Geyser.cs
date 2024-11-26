using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    public float liftForce = 30f; // Strength of the lift force

    private void OnTriggerStay(Collider other)
    {
        // Check if the object entering the geyser area has a Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Apply an upward force to the ball
            Vector3 upwardForce = Vector3.up * liftForce;
            rb.AddForce(upwardForce, ForceMode.Acceleration);
        }
    }
}
