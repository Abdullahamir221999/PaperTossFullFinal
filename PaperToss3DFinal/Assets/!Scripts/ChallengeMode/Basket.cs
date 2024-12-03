/*using UnityEngine;
using System.Collections;

public class Basket : MonoBehaviour
{
    public Transform[] basketPositions;  // Assign the positions in the Inspector
    public float moveDuration = 1.0f;    // How long it takes to move to a new position

    private Transform currentPosition;

    private void Start()
    {
        // Set the initial position of the basket
        MoveBasketToRandomPosition();
    }

    public void MoveBasketToRandomPosition()
    {
        // Choose a random position different from the current one
        Transform newPosition;
        do
        {
            newPosition = basketPositions[Random.Range(0, basketPositions.Length)];
        }
        while (newPosition == currentPosition);

        // Move the basket to the new position
        currentPosition = newPosition;
        StartCoroutine(MoveBasket());
    }

    private IEnumerator MoveBasket()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = currentPosition.position;
        Vector3 originalScale = transform.localScale;  // Preserve the scale
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            transform.localScale = originalScale;  // Maintain the original scale
            yield return null;
        }

        transform.position = targetPosition;  // Ensure exact final position
        transform.localScale = originalScale; // Ensure exact final scale
    }
}
*/
using UnityEngine;
using System.Collections;

public class Basket : MonoBehaviour
{
    public Transform[] basketPositions;  // Assign the positions in the Inspector
    public float moveDuration = 2.0f;    // How long it takes to move to a new position

    private Transform currentPosition;

    private void Start()
    {
        // Set the initial position of the basket
        MoveBasketToRandomPosition();
    }

    public void MoveBasketToRandomPosition()
    {
        // Choose a random position different from the current one
        Transform newPosition;
        do
        {
            newPosition = basketPositions[Random.Range(0, basketPositions.Length)];
        }
        while (newPosition == currentPosition);

        // Move the basket to the new position
        currentPosition = newPosition;
        StartCoroutine(MoveBasket());
    }

    private IEnumerator MoveBasket()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = currentPosition.position;
        Vector3 originalScale = transform.localScale;  // Preserve the scale
        float elapsedTime = 0;

        // Get the MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        while (elapsedTime < moveDuration)
        {
            // Move both the basket's transform and the mesh
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            if (meshFilter != null)
            {
                // Update the mesh's position too
                meshFilter.transform.position = transform.position;
            }

            elapsedTime += Time.deltaTime;
            transform.localScale = originalScale;  // Maintain the original scale
            yield return null;
        }

        // Ensure exact final position
        transform.position = targetPosition;
        if (meshFilter != null)
        {
            meshFilter.transform.position = targetPosition;
        }

        // Ensure exact final scale
        transform.localScale = originalScale;
    }
}
