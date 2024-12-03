using UnityEngine;

public class FanZone : MonoBehaviour
{
    [Header("Fan Settings")]
    public Vector3 fanDirection = Vector3.forward; // Direction of the fan's force
    public float fanForce = 10f; // Strength of the fan's force
    public bool windBlocked = false; // Reference from power-up script to block wind

    private void OnTriggerStay(Collider other)
    {
        // Check if the wind is blocked, exit if it is
        if (windBlocked) return;

        // Check if the object has a Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Apply force to the Rigidbody in the direction of the fan
            rb.AddForce(fanDirection.normalized * fanForce, ForceMode.Acceleration);
        }
    }
}

