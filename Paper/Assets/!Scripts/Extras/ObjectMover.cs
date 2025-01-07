using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public float speed = 2.0f; // Speed of movement
    public float range = 2.0f; // Range of movement

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new position using a sine wave function for smooth back and forth movement
        float newPosition = Mathf.Sin(Time.time * speed) * range;
        transform.position = startPosition + new Vector3(newPosition, 0, 0);
    }
}
