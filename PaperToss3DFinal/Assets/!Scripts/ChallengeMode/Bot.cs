using UnityEngine;

public class Bot : MonoBehaviour
{
    public float timeBetweenTosses = 2f; // Time delay between tosses
    public float tossAccuracy = 0.8f; // 80% chance to make a basket
    private float tossTimer;

    private void Update()
    {
        tossTimer += Time.deltaTime;

        if (tossTimer >= timeBetweenTosses)
        {
            TossBall();
            tossTimer = 0f; // Reset timer
        }
    }

    private void TossBall()
    {
        // Randomize bot success
        bool success = Random.value < tossAccuracy;

        if (success)
        {
            GameManager.Instance.MadeBucket(); // Increment bot score
        }
        else
        {
            GameManager.Instance.MissedBucket(); // Bot missed the shot
        }
    }
}
