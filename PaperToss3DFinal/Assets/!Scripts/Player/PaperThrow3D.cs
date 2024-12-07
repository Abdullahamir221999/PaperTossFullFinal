using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PaperThrow3D : MonoBehaviour
{
    
    public ShopData ballData;
    
    private Vector3 startTouchPosition, endTouchPosition;
    private float startTime, endTime;
    private Rigidbody rb;
    public float minThrowForce = 5f;
    public float maxThrowForce = 15f;
    private bool isThrown = false;
    private Vector3 initialPosition;

    private int attempts = 0;
    public int maxAttempts = 5; // Not const anymore

    private int comboCount = 0; // To track combo count

    public float resetTime = 2f;
    public float endLevelDelay = 2f;

    private bool hitBin = false;
    public Transform binTransform;
    public ParticleSystem successParticles;

    public AudioClip throwSound;
    public AudioClip[] hitBinSounds; // Array of different hit sounds
    public AudioClip missSound; // Sound for missed shot
    private AudioSource audioSource;

    //glider powerup
    public float gliderMinThrowForce = 10f; // Adjust as needed
    public float gliderMaxThrowForce = 25f; // Adjust as needed
    private bool isGliderActive = false;

    //magnet 
    public bool basketMagnetActive = false;

    void Start()
    {
        ballData = Resources.Load<ShopData>("BallData");
        Renderer rend = GetComponent<Renderer>();
        foreach (var BallData in ballData.shopItems)
        {
            if (BallData.ballState == BallState.Equipped)
            {
                rend.material = BallData.ballMaterial;
            }
        }
        
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        ResetBall();

        // Adjust maxAttempts based on challenge mode
        if (GameManager.Instance.isChallengeMode)
        {
            maxAttempts = int.MaxValue; // Effectively unlimited attempts
        }
        else
        {
            maxAttempts = 3; // Regular mode limit
        }

        if (GameManager.Instance.isUnlimitedMode)
        {
            maxAttempts = 1000;
        }
        // Assign binTransform based on the "Bucket" tag
        GameObject bucket = GameObject.FindGameObjectWithTag("Bucket");
        if (bucket != null)
        {
            binTransform = bucket.transform;
            Debug.Log("Bucket assigned to binTransform.");
        }
        else
        {
            Debug.LogWarning("No bucket found with the 'Bucket' tag in this level.");
        }
    }

    void FixedUpdate()
    {
        // Check if the paper is in the air and magnet is active
        if (isThrown && basketMagnetActive)
        {
            ApplyBasketMagnetForce();
        }
    }
    /*void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isThrown && Input.mousePosition.y <= Screen.height / 2)
        {
            startTouchPosition = Input.mousePosition;
            startTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0) && !isThrown && startTouchPosition.y <= Screen.height / 2)
        {
            endTouchPosition = Input.mousePosition;
            endTime = Time.time;

            Vector3 swipe = endTouchPosition - startTouchPosition;
            float duration = endTime - startTime;

            if (swipe.magnitude > 0 && duration > 0)
            {
                ThrowBall(swipe, duration);
            }
            else
            {
                Debug.LogWarning("Invalid swipe detected. Swipe magnitude or duration is too small.");
            }
        }
    }*/
    void Update()
    {
        // Check if the pointer is over a UI element to prevent accidental throws
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Ignore input if over a UI element
        }

        if (Input.GetMouseButtonDown(0) && !isThrown && Input.mousePosition.y <= Screen.height / 2)
        {
            startTouchPosition = Input.mousePosition;
            startTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0) && !isThrown && startTouchPosition.y <= Screen.height / 2)
        {
            endTouchPosition = Input.mousePosition;
            endTime = Time.time;

            Vector3 swipe = endTouchPosition - startTouchPosition;
            float duration = endTime - startTime;

            if (swipe.magnitude > 0 && duration > 0)
            {
                ThrowBall(swipe, duration);
            }
            else
            {
                Debug.LogWarning("Invalid swipe detected. Swipe magnitude or duration is too small.");
            }
        }
    }


    void ThrowBall(Vector3 swipe, float duration)
    {
        Vector3 throwDirection = swipe.normalized;
        float throwStrength = Mathf.Clamp(swipe.magnitude / (duration * 200),
                                          isGliderActive ? gliderMinThrowForce : minThrowForce,
                                          isGliderActive ? gliderMaxThrowForce : maxThrowForce);

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce((Camera.main.transform.forward + throwDirection) * throwStrength, ForceMode.Impulse);

        if (throwSound != null)
        {
            audioSource.PlayOneShot(throwSound);
        }

        isThrown = true;

        // Only increment attempts in normal mode
        if (!GameManager.Instance.isChallengeMode)
        {
            attempts++;
            if (GameManager.Instance.isUnlimitedMode) return;
            if (attempts >= maxAttempts)
            {
                StartCoroutine(EndLevelWithDelay());
            }
        }
    }

    public void ActivatePaperGlider(float duration)
    {
        Debug.Log("Paper Glider Power-Up Activated");
        isGliderActive = true;
        Invoke("DeactivatePaperGlider", duration); // Set a timer to turn it off
    }

    private void DeactivatePaperGlider()
    {
        isGliderActive = false;
    }
    void ApplyBasketMagnetForce()
    {
        // Calculate the direction toward the basket
        Vector3 magnetDirection = (binTransform.position - transform.position).normalized;
        float magnetForce = 7f; // Adjust this to control the strength of the magnet effect

        // Apply a force toward the basket
        rb.AddForce(magnetDirection * magnetForce, ForceMode.Acceleration);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Bin") && !hitBin)
        {
            hitBin = true;
            comboCount++; // Increment combo count on successful hit

            // Play a hit sound based on combo count
            PlayHitSound();

            GameManager.Instance.MadeBucket();
            successParticles.Play();
            AnimateBin();
            StartCoroutine(ResetBallAfterDelay());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            comboCount = 0; // Reset combo count on miss
            PlayMissSound(); // Play miss sound
            GameManager.Instance.MissedBucket();
            StartCoroutine(ResetBallAfterDelay());
        }
    }

    IEnumerator EndLevelWithDelay()
    {
        yield return new WaitForSeconds(endLevelDelay);

        // Only end the level in normal mode
        if (!GameManager.Instance.isChallengeMode)
        {
            GameManager.Instance.FinishGame();
        }
    }

    IEnumerator ResetBallAfterDelay()
    {
        yield return new WaitForSeconds(resetTime);

        // Reset the ball regardless of attempts in challenge mode
        if (GameManager.Instance.isChallengeMode || attempts < maxAttempts)
        {
            ResetBall();
        }
    }

    void ResetBall()
    {
        /*rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;*/
        rb.isKinematic = true;
        isThrown = false;
        hitBin = false;
        transform.position = initialPosition;
    }

    void PlayHitSound()
    {
        // Play different sound for each combo level
        int soundIndex = (comboCount - 1) % hitBinSounds.Length;
        if (hitBinSounds[soundIndex] != null)
        {
            audioSource.PlayOneShot(hitBinSounds[soundIndex]);
        }
    }

    void PlayMissSound()
    {
        if (missSound != null)
        {
            audioSource.PlayOneShot(missSound);
        }
    }

    void AnimateBin()
    {
        if (binTransform == null)
        {
            Debug.LogWarning("binTransform is not assigned. Ensure the bucket has the correct tag.");
            return;
        }

        Sequence binSequence = DOTween.Sequence();

        binSequence.Append(binTransform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.15f).SetEase(Ease.InOutQuad))
                   .Append(binTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.2f).SetEase(Ease.InOutQuad))
                   .Append(binTransform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).SetEase(Ease.OutBounce)); // Final scale is set to (1.25, 1.25, 1.25)

        binSequence.Play();
        Debug.Log("AnimateBin sequence played on: " + binTransform.name);
    }



}
