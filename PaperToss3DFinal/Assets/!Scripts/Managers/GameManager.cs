
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
    public bool isUnlimitedMode = false;  // Toggle to check if Unlimited mode is active
    public float challengeTimer = 120f;   // Timer in seconds, changeable from the Inspector
    private float currentTimer;
    private bool timerStarted = false; // Flag to check if timer has started

    public TextMeshProUGUI timerText; // UI text to display the timer
    public TextMeshProUGUI basketsMadeText; // UI text to display baskets made count

    public TextMeshProUGUI coinText;

    public PowerUpController powerUpController;

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
        if(isUnlimitedMode) return;
        
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
