/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxTries = 5;
    private int currentTries = 0;
    private int bucketsMade = 0;
    private bool hasBaggedAtLeastOne = false;  // Track if at least one bucket was made

    public GameObject winPanel;
    public GameObject losePanel; // Lose panel for when the player doesn't bag any bins
    public Button nextLevelButton;
    public Button mainMenuButton;
    public Button retryButton;
    public Basket basket;

    public Button loseMainMenuButton;
    public Button loseRetryButton;

    public Button pauseMainMenuButton;
    public Button pauseRetryButton;

    public GameObject[] starImages;

    public bool isChallengeMode = false;  // Toggle to check if challenge mode is active
    public float challengeTimer = 120f;   // Timer in seconds, changeable from the Inspector
    private float currentTimer;
    private bool timerStarted = false; // Flag to check if timer has started

    public TextMeshProUGUI timerText; // UI text to display the timer
    public TextMeshProUGUI basketsMadeText; // UI text to display baskets made count

    public TextMeshProUGUI coinText;

    public PowerUpController powerUpController;

    [Header("Unlimited Mode Settings")]
    public bool isUnlimitedMode = false;  // Toggle to check if Unlimited mode is active
    public float unlimitedTimer = 30f;    // Starting time for Unlimited Mode
    public float timePerToss = 10f;       // Time added per successful toss
    public float timePenalty = 3f;        // Time deducted per missed toss
    private int streakCount = 0;          // Successful toss streak
    private bool isUnlimitedGameOver = false;


    private bool isTaseer = true;
    private void OnEnable()
    {
        Time.timeScale = 1;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        nextLevelButton.onClick.AddListener(LoadNextLevel);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        retryButton.onClick.AddListener(RetryLevel);

        loseMainMenuButton.onClick.AddListener(LoadMainMenu);
        loseRetryButton.onClick.AddListener(RetryLevel);

        pauseMainMenuButton.onClick.AddListener(LoadMainMenu);
        pauseRetryButton.onClick.AddListener(RetryLevel);
        if (PlayerPrefs.GetInt("ChallengeMode") == 1)
        {
            isChallengeMode = true;
            isUnlimitedMode = false;
        }
        else if (PlayerPrefs.GetInt("UnlimitedMode") == 1)
        {
            isChallengeMode = false;
            isUnlimitedMode = true;
        }
        // Initialize the timer for challenge mode
        if (isChallengeMode)
        {
            currentTimer = challengeTimer;
            UpdateTimerUI(); // Display the initial timer
        }
        if (isUnlimitedMode)
        {
            maxTries = 1000;
            currentTimer = 1000000;
            UpdateTimerUI();
        }
    }

    private void Update()
    {
        // Only run the timer if challenge mode is active and the timer has started
        if (isChallengeMode && timerStarted)
        {
            currentTimer -= Time.deltaTime;
            UpdateTimerUI(); // Update the timer UI every frame

            if (currentTimer <= 0 && isTaseer)
            {
                isTaseer = false;
                currentTimer = 0;
                FinishGame(); // End the game when the timer runs out
            }
        }
    }

    // Updates the timer UI text
    private void UpdateTimerUI()
    {
        if (isUnlimitedMode)
        {
            return;
        }

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTimer / 60F);
            int seconds = Mathf.FloorToInt(currentTimer % 60F);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    public void MadeBucket()
    {
        bucketsMade++;
        hasBaggedAtLeastOne = true;  // Mark that the player has successfully bagged a bin

        // Move basket only if challenge mode is active
        if (isChallengeMode)
        {
            if (basket != null)
            {
                basket.MoveBasketToRandomPosition();
            }
            else
            {
                Debug.LogWarning("Basket is not assigned in Challenge Mode!");
            }

            // Start the timer only after the first successful basket in challenge mode
            if (!timerStarted)
            {
                StartChallengeTimer();
            }
        }

        // In normal mode, baskets do not move
        if (!isChallengeMode)
        {
            currentTries++;
        }

        UpdateUI();
    }

    public void MissedBucket()
    {
        // Only increase tries in normal mode
        if (!isChallengeMode)
        {
            currentTries++;
        }

        UpdateUI();
    }

    private void StartChallengeTimer()
    {
        timerStarted = true; // Start the timer
    }

    private void UpdateUI()
    {
        // Update any UI elements showing current tries, buckets made, or the timer
    }

    public void FinishGame()
    {
        if (isUnlimitedMode) return;

        if (hasBaggedAtLeastOne)
        {
            // Player made at least one bucket, show win panel
            UpdateCoins(); // Ensure UI shows the latest coin count
            winPanel.SetActive(true);
            int starsEarned = bucketsMade;
            UpdateStarsUI(starsEarned);

            // Award coins based on stars earned
            switch (starsEarned)
            {
                case 1:
                    SetCoins(150);
                    break;
                case 2:
                    SetCoins(300);
                    break;
                case 3:
                    SetCoins(500);
                    break;
                case 4:
                    SetCoins(600);
                    break;
                case >= 5:
                    SetCoins(1000);
                    break;
                default:
                    Debug.Log("No stars earned.");
                    break;
            }

            PlayerProgress.Instance.UpdateStarsForLevel(PlayerProgress.Instance.currentLevel, starsEarned);

            // Show or hide basket count based on mode
            if (isChallengeMode && basketsMadeText != null)
            {
                basketsMadeText.text = "Baskets Made: " + bucketsMade.ToString();
                basketsMadeText.gameObject.SetActive(true);
            }
            else if (basketsMadeText != null)
            {
                basketsMadeText.gameObject.SetActive(false);
            }

            if (!isChallengeMode)
            {
                PlayerProgress.Instance.UnlockNextLevel();  // Unlock the next level in normal mode
            }

            PlayerProgress.Instance.SaveProgress();
        }
        else
        {
            // Player didn't make any bucket, show lose panel
            losePanel.SetActive(true);
        }
    }

    private void SetCoins(int coins)
    {
        if (powerUpController != null && powerUpController.scoreMultiplierActive)
        {
            Debug.Log("Score Multiplier is active. Original coins: " + coins);
            coins *= 2; // Double the coins if score multiplier is active
            Debug.Log("Coins after multiplier: " + coins);
        }
        else
        {
            Debug.Log("Score Multiplier is not active. Coins: " + coins);
        }

        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coins);
        UpdateCoins(); // Refresh displayed coin count
    }


    private void UpdateStarsUI(int starsEarned)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].SetActive(i < starsEarned);
        }
    }

    private void LoadNextLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("LevelManager instance is null!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    private void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void RetryLevel()
    {
        LevelManager.Instance.LoadLevel(PlayerProgress.Instance.currentLevel);
    }
    private void UpdateCoins()
    {
        coinText.text = PlayerPrefs.GetInt("Coins").ToString();
    }
    public void GrantExtraLife()
    {
        // Find the PaperThrow3D instance and increase its maxAttempts to 4
        PaperThrow3D paperThrow = FindObjectOfType<PaperThrow3D>();
        if (paperThrow != null)
        {
            paperThrow.maxAttempts = 4;
            Debug.Log("Extra Life granted: maxAttempts set to 4.");
        }
    }

}

*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxTries = 5;
    private int currentTries = 0;
    private int bucketsMade = 0;
    private bool hasBaggedAtLeastOne = false;  // Track if at least one bucket was made

    public GameObject winPanel;
    public GameObject losePanel; // Lose panel for when the player doesn't bag any bins
    public Button nextLevelButton;
    public Button mainMenuButton;
    public Button retryButton;
    public Basket basket;

    public Button loseMainMenuButton;
    public Button loseRetryButton;

    public Button pauseMainMenuButton;
    public Button pauseRetryButton;

    public GameObject[] starImages;

    public bool isChallengeMode = false;  // Toggle to check if challenge mode is active
    public float challengeTimer = 120f;   // Timer in seconds, changeable from the Inspector
    private float currentTimer;
    private bool timerStarted = false; // Flag to check if timer has started

    public TextMeshProUGUI timerText; // UI text to display the timer
    public TextMeshProUGUI basketsMadeText; // UI text to display baskets made count

    public TextMeshProUGUI coinText;

    public PowerUpController powerUpController;

    [Header("Unlimited Mode Settings")]
    public bool isUnlimitedMode = false;  // Toggle to check if Unlimited mode is active
    public float unlimitedTimer = 30f;    // Starting time for Unlimited Mode
    public float timePerToss = 10f;       // Time added per successful toss
    public float timePenalty = 3f;        // Time deducted per missed toss
    private int streakCount = 0;          // Successful toss streak
    private bool isUnlimitedGameOver = false;


    private bool isTaseer = true;
    private void OnEnable()
    {
        Time.timeScale = 1;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*private void Start()
    {
        isChallengeMode = false;
        isUnlimitedMode = false;
        nextLevelButton.onClick.AddListener(LoadNextLevel);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        retryButton.onClick.AddListener(RetryLevel);

        loseMainMenuButton.onClick.AddListener(LoadMainMenu);
        loseRetryButton.onClick.AddListener(RetryLevel);

        pauseMainMenuButton.onClick.AddListener(LoadMainMenu);
        pauseRetryButton.onClick.AddListener(RetryLevel);
        if (PlayerPrefs.GetInt("ChallengeMode") == 1)
        {
            isChallengeMode = true;
        }
        else if (PlayerPrefs.GetInt("UnlimitedMode") == 1)
        {
            isUnlimitedMode = true;
        }
        // Initialize the timer for challenge mode
        if (isChallengeMode)
        {
            currentTimer = challengeTimer;
             // Display the initial timer
        }
        else if (isUnlimitedMode)
        {
            currentTimer = unlimitedTimer;
            streakCount = 0;
        }
        UpdateTimerUI();
    }*/
    private void Start()
    {
        // Reset both modes to false initially
        isChallengeMode = false;
        isUnlimitedMode = false;

        // Set mode based on PlayerPrefs
        if (PlayerPrefs.GetInt("ChallengeMode", 0) == 1)
        {
            isChallengeMode = true;
        }
        else if (PlayerPrefs.GetInt("UnlimitedMode", 0) == 1)
        {
            isUnlimitedMode = true;
        }

        Debug.Log($"Challenge Mode: {isChallengeMode}, Unlimited Mode: {isUnlimitedMode}");

        // Initialize UI and timers based on the selected mode
        if (isChallengeMode)
        {
            currentTimer = challengeTimer;
            UpdateTimerUI();
        }
        else if (isUnlimitedMode)
        {
            maxTries = 1000;
            currentTimer = 30f;  // Initial time for Unlimited Mode
            UpdateTimerUI();
        }
    }


    private void Update()
    {
        // Only run the timer if challenge mode is active and the timer has started
        if (isChallengeMode && timerStarted)
        {
            currentTimer -= Time.deltaTime;
            UpdateTimerUI(); // Update the timer UI every frame

            if (currentTimer <= 0 && isTaseer)
            {
                isTaseer = false;
                currentTimer = 0;
                FinishGame(); // End the game when the timer runs out
            }
        }
        else if (isUnlimitedMode && !isUnlimitedGameOver)
        {
            currentTimer -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTimer <= 0)
            {
                currentTimer = 0;
                isUnlimitedGameOver = true;
                FinishUnlimitedGame();
            }
        }
    }

    // Updates the timer UI text
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTimer / 60F);
            int seconds = Mathf.FloorToInt(currentTimer % 60F);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    public void MadeBucket()
    {
        bucketsMade++;
        hasBaggedAtLeastOne = true;  // Mark that the player has successfully bagged a bin

        // Move basket only if challenge mode is active
        if (isChallengeMode)
        {
            if (basket != null)
            {
                basket.MoveBasketToRandomPosition();
            }
            else
            {
                Debug.LogWarning("Basket is not assigned in Challenge Mode!");
            }

            // Start the timer only after the first successful basket in challenge mode
            if (!timerStarted)
            {
                StartChallengeTimer();
            }
        }
        if (isUnlimitedMode)
        {
            streakCount++;
            currentTimer += timePerToss;
            Debug.Log($"Streak: {streakCount} | Timer: {currentTimer}");

            if (streakCount % 5 == 0)
            {
                IncreaseUnlimitedDifficulty();
            }
        }

        // In normal mode, baskets do not move
        if (!isChallengeMode)
        {
            currentTries++;
        }

        UpdateUI();
    }
    public void IncreaseUnlimitedDifficulty()
    {
        Debug.Log("Increasing difficulty now");

        // Increase wind force or obstacles if you have a FanZone or other obstacles
        FanZone fanZone = FindObjectOfType<FanZone>();
        if (fanZone != null)
        {
            fanZone.fanForce += 0.5f; // Increase wind strength
            Debug.Log("Wind force increased to: " + fanZone.fanForce);
        }

        // Increase the basket movement speed if there's a basket component
        Basket basket = FindObjectOfType<Basket>();
        if (basket != null)
        {
            basket.MoveBasketToRandomPosition();
            basket.moveSpeed += 1f; // Increase basket movement speed
            Debug.Log("Basket move speed increased to: " + basket.moveSpeed);
        }

        // Optionally, you can decrease the time added per successful toss
        if (timePerToss > 1f)
        {
            timePerToss -= 0.5f; // Decrease time added with each successful toss
            Debug.Log("Time added per toss decreased to: " + timePerToss);
        }

        /*// Optional: Increase the ball's throwing force, making it harder to control
        float increaseThrowForce = 1.5f;
        if (minThrowForce < 20f)
        {
            minThrowForce *= increaseThrowForce;
            maxThrowForce *= increaseThrowForce;
            Debug.Log("Ball throwing force increased to: " + minThrowForce + " - " + maxThrowForce);
        }*/
    }
    public void MissedBucket()
    {
        // Only increase tries in normal mode
        if (!isChallengeMode)
        {
            currentTries++;
        }
        if (isUnlimitedMode)
        {
            currentTimer -= timePenalty;
            Debug.Log($"Missed! Timer: {currentTimer}");
        }

        UpdateUI();
    }
    private void FinishUnlimitedGame()
    {
        Debug.Log("Unlimited Mode Over!");
        losePanel.SetActive(true);
        basketsMadeText.text = $"Final Streak: {streakCount}";
    }

    private void StartChallengeTimer()
    {
        timerStarted = true; // Start the timer
    }

    private void UpdateUI()
    {
        // Update any UI elements showing current tries, buckets made, or the timer
    }

    public void FinishGame()
    {
        if (isUnlimitedMode) return;

        if (hasBaggedAtLeastOne)
        {
            // Player made at least one bucket, show win panel
            UpdateCoins(); // Ensure UI shows the latest coin count
            winPanel.SetActive(true);
            int starsEarned = bucketsMade;
            UpdateStarsUI(starsEarned);

            // Award coins based on stars earned
            switch (starsEarned)
            {
                case 1:
                    SetCoins(150);
                    break;
                case 2:
                    SetCoins(300);
                    break;
                case 3:
                    SetCoins(500);
                    break;
                case 4:
                    SetCoins(600);
                    break;
                case >= 5:
                    SetCoins(1000);
                    break;
                default:
                    Debug.Log("No stars earned.");
                    break;
            }

            PlayerProgress.Instance.UpdateStarsForLevel(PlayerProgress.Instance.currentLevel, starsEarned);

            // Show or hide basket count based on mode
            if (isChallengeMode && basketsMadeText != null)
            {
                basketsMadeText.text = "Baskets Made: " + bucketsMade.ToString();
                basketsMadeText.gameObject.SetActive(true);
            }
            else if (basketsMadeText != null)
            {
                basketsMadeText.gameObject.SetActive(false);
            }

            if (!isChallengeMode)
            {
                PlayerProgress.Instance.UnlockNextLevel();  // Unlock the next level in normal mode
            }

            PlayerProgress.Instance.SaveProgress();
        }
        else
        {
            // Player didn't make any bucket, show lose panel
            losePanel.SetActive(true);
        }
    }

    private void SetCoins(int coins)
    {
        if (powerUpController != null && powerUpController.scoreMultiplierActive)
        {
            Debug.Log("Score Multiplier is active. Original coins: " + coins);
            coins *= 2; // Double the coins if score multiplier is active
            Debug.Log("Coins after multiplier: " + coins);
        }
        else
        {
            Debug.Log("Score Multiplier is not active. Coins: " + coins);
        }

        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coins);
        UpdateCoins(); // Refresh displayed coin count
    }


    private void UpdateStarsUI(int starsEarned)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].SetActive(i < starsEarned);
        }
    }

    private void LoadNextLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("LevelManager instance is null!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    private void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void RetryLevel()
    {
        LevelManager.Instance.LoadLevel(PlayerProgress.Instance.currentLevel);
    }
    private void UpdateCoins()
    {
        coinText.text = PlayerPrefs.GetInt("Coins").ToString();
    }
    public void GrantExtraLife()
    {
        // Find the PaperThrow3D instance and increase its maxAttempts to 4
        PaperThrow3D paperThrow = FindObjectOfType<PaperThrow3D>();
        if (paperThrow != null)
        {
            paperThrow.maxAttempts = 4;
            Debug.Log("Extra Life granted: maxAttempts set to 4.");
        }
    }

}

