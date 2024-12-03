using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Needed for scene management

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button challengeModeButton; // New button for challenge mode
    public TextMeshProUGUI totalStarsText; // UI Text to display total stars
    public GameObject[] levelButtons;      // Array of level buttons or indicators

    private void Start()
    {
        Debug.Log("MainMenu Start method called");

        if (playButton == null)
        {
            Debug.LogError("Play button is not assigned in the Inspector!");
            return;
        }

        if (challengeModeButton == null)
        {
            Debug.LogError("Challenge Mode button is not assigned in the Inspector!");
            return;
        }

        playButton.onClick.AddListener(StartGame);
        challengeModeButton.onClick.AddListener(StartChallengeMode); // Add listener for Challenge Mode button

        Debug.Log("Play and Challenge Mode button listeners added successfully");

        // Check if LevelManager exists
        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelManager instance is null! Creating a new LevelManager.");
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManagerObj.AddComponent<LevelManager>();
        }

        // Update UI to display stars
        UpdateStarsDisplay();
    }

    private void UpdateStarsDisplay()
    {
        Debug.Log("UpdateStarsDisplay method called");

        int totalStars = PlayerProgress.Instance.GetTotalStars();
        Debug.Log("Total Stars from PlayerProgress: " + totalStars);
        totalStarsText.text = "Total Stars: " + totalStars.ToString();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int starsForLevel = PlayerProgress.Instance.GetStarsForLevel(i + 1); // Levels are 1-indexed
            Debug.Log($"Stars for Level {i + 1}: {starsForLevel}");

            TextMeshProUGUI starText = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (starText != null)
            {
                starText.text = starsForLevel.ToString();
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame method called");

        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelManager instance is still null! Unable to start the game.");
            return;
        }

        LevelManager.Instance.LoadLevel(1); // Always start from level 1
    }

    public void StartChallengeMode()
    {
        Debug.Log("StartChallengeMode method called");
        SceneManager.LoadScene("ChallengeMode"); // Replace "ChallengeMode" with your challenge scene name
    }
}
