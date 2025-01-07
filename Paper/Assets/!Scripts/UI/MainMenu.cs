/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Needed for scene management

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button challengeModeButton; // New button for challenge mode
    public Button unlimitedModeButton; // New button for unlimited mode
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
        unlimitedModeButton.onClick.AddListener(UnlimitedMode);
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
        PlayerPrefs.SetInt("ChallengeMode", 1);
        SceneManager.LoadScene("ChallengeMode"); // Replace "ChallengeMode" with your challenge scene name
    }
    
    public void UnlimitedMode(){
        Debug.Log("UnlimitedMode method called");
        PlayerPrefs.SetInt("UnlimitedMode", 1);
        SceneManager.LoadScene("UnlimitedMode");    
    }
    
}
*/
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Needed for scene management

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button challengeModeButton; // Button for challenge mode
    public Button unlimitedModeButton; // Button for unlimited mode
    public TextMeshProUGUI totalStarsText; // UI Text to display total stars
    public GameObject[] levelButtons; // Array of level buttons or indicators

    private void Start()
    {
        Debug.Log("MainMenu Start method called");

        // Check and log errors for unassigned buttons
        if (playButton == null)
        {
            Debug.LogError("Play button is not assigned in the Inspector!");
        }
        else
        {
            playButton.onClick.AddListener(StartGame);
        }

        if (challengeModeButton == null)
        {
            Debug.LogError("Challenge Mode button is not assigned in the Inspector!");
        }
        else
        {
            challengeModeButton.onClick.AddListener(StartChallengeMode);
        }

        if (unlimitedModeButton == null)
        {
            Debug.LogError("Unlimited Mode button is not assigned in the Inspector!");
        }
        else
        {
            unlimitedModeButton.onClick.AddListener(StartUnlimitedMode);
        }

        Debug.Log("Button listeners added successfully");

        // Ensure LevelManager is present
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

        // Ensure no mode is selected
        PlayerPrefs.SetInt("ChallengeMode", 0);
        PlayerPrefs.SetInt("UnlimitedMode", 0);
        PlayerPrefs.Save();

        LevelManager.Instance.LoadLevel(1); // Always start from level 1
    }

    public void StartChallengeMode()
    {
        Debug.Log("StartChallengeMode method called");

        // Set ChallengeMode to 1 and UnlimitedMode to 0
        PlayerPrefs.SetInt("ChallengeMode", 1);
        PlayerPrefs.SetInt("UnlimitedMode", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("ChallengeMode"); // Replace with your challenge mode scene name
    }

    public void StartUnlimitedMode()
    {
        Debug.Log("StartUnlimitedMode method called");

        // Set UnlimitedMode to 1 and ChallengeMode to 0
        PlayerPrefs.SetInt("UnlimitedMode", 1);
        PlayerPrefs.SetInt("ChallengeMode", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("UnlimitedMode"); // Replace with your unlimited mode scene name
    }
}
