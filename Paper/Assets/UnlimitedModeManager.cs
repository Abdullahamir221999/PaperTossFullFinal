/*using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlimitedModeManager : MonoBehaviour
{
    public static UnlimitedModeManager Instance { get; private set; }

    [Header("Unlimited Mode Settings")]
    public float unlimitedTimer = 30f;   // Starting time for Unlimited Mode
    public float timePerToss = 5f;       // Time added per successful toss
    public float timePenalty = 3f;       // Time deducted per missed toss
    private int streakCount = 0;
    private bool isGameOver = false;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI basketsMadeText;
    public TextMeshProUGUI coinText;
    public GameObject losePanel;
    public Button mainMenuButton;
    public Button retryButton;

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
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        retryButton.onClick.AddListener(RetryLevel);

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            unlimitedTimer -= Time.deltaTime;
            unlimitedTimer = Mathf.Max(unlimitedTimer, 0);
            UpdateTimerUI();

            if (unlimitedTimer <= 0)
            {
                GameOver();
            }
        }
    }

    public void MadeBucket()
    {
        streakCount++;
        unlimitedTimer += timePerToss;
        UpdateTimerUI();
        UpdateBasketsMadeUI();

        if (streakCount % 5 == 0)
        {
            IncreaseDifficulty();
        }
    }

    public void MissedBucket()
    {
        unlimitedTimer -= timePenalty;
        UpdateTimerUI();

        if (unlimitedTimer <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        losePanel.SetActive(true);
        basketsMadeText.text = $"Final Streak: {streakCount}";
    }

    private void IncreaseDifficulty()
    {
        FanZone fanZone = FindObjectOfType<FanZone>();
        if (fanZone != null)
        {
            fanZone.fanForce += 0.5f;
            Debug.Log("Wind Strength Increased!");
        }

        Basket basket = FindObjectOfType<Basket>();
        if (basket != null)
        {
            basket.MoveBasketToRandomPosition();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(unlimitedTimer / 60F);
            int seconds = Mathf.FloorToInt(unlimitedTimer % 60F);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateBasketsMadeUI()
    {
        if (basketsMadeText != null)
        {
            basketsMadeText.text = $"Streak: {streakCount}";
        }
    }

    private void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void RetryLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UnlimitedModeScene");
    }
}
*/
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UnlimitedModeManager : MonoBehaviour
{
    public static UnlimitedModeManager Instance { get; private set; }

    [Header("Unlimited Mode Settings")]
    public float unlimitedTimer = 30f;   // Starting time for Unlimited Mode
    public float timePerToss = 10f;       // Time added per successful toss
    public float timePenalty = 3f;       // Time deducted per missed toss
    private int streakCount = 0;
    private bool isGameOver = false;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI basketsMadeText;
    public TextMeshProUGUI coinText;
    public GameObject losePanel;
    public GameObject pausePanel;
    public Button mainMenuButton;
    public Button retryButton;
    public Button pauseButton;
    public Button resumeButton;

    [Header("References")]
    public PaperThrow3D paperThrowScript;

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
        // Button Listeners
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        retryButton.onClick.AddListener(RetryLevel);
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            unlimitedTimer -= Time.deltaTime;
            unlimitedTimer = Mathf.Max(unlimitedTimer, 0);
            UpdateTimerUI();

            if (unlimitedTimer <= 0)
            {
                GameOver();
            }
        }
    }

    public void MadeBucket()
    {
        Debug.Log("I made a bucket");
        if (isGameOver) return;

        Debug.Log($"Timer before adding: {unlimitedTimer}");
        streakCount++;
        unlimitedTimer += timePerToss;
        Debug.Log($"Timer after adding {timePerToss} seconds: {unlimitedTimer}");

        UpdateTimerUI();
        UpdateBasketsMadeUI();

        if (streakCount % 5 == 0)
        {
            IncreaseDifficulty();
        }

        if (paperThrowScript != null)
        {
            paperThrowScript.ResetBall();
        }
    }


    public void MissedBucket()
    {
        if (isGameOver) return;

        unlimitedTimer -= timePenalty;
        UpdateTimerUI();

        Debug.Log($"Missed! Timer: {unlimitedTimer}");

        // Reset the ball for the next throw
        if (paperThrowScript != null)
        {
            paperThrowScript.ResetBall();
        }

        if (unlimitedTimer <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        losePanel.SetActive(true);
        basketsMadeText.text = $"Final Streak: {streakCount}";
    }

    private void IncreaseDifficulty()
    {
        FanZone fanZone = FindObjectOfType<FanZone>();
        if (fanZone != null)
        {
            fanZone.fanForce += 0.5f;
            Debug.Log("Wind Strength Increased!");
        }

        Basket basket = FindObjectOfType<Basket>();
        if (basket != null)
        {
            basket.MoveBasketToRandomPosition();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(unlimitedTimer / 60F);
            int seconds = Mathf.FloorToInt(unlimitedTimer % 60F);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        else
        {
            Debug.LogError("timerText is not assigned!");
        }
    }


    private void UpdateBasketsMadeUI()
    {
        if (basketsMadeText != null)
        {
            basketsMadeText.text = $"Streak: {streakCount}";
        }
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
}
