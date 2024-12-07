using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Button playButton; // Assign this in the Inspector
    public Button startButton; // Assign this in the Inspector
    public Button skinsButton; // Assign this in the Inspector
    public Button storeButton; // Assign this in the Inspector

    public GameObject skinsPanel; // Assign this in the Inspector
    public GameObject storePanel; // Assign this in the Inspector

    public Button skinsCloseButton; // Assign this in the Inspector
    public Button storeCloseButton; // Assign this in the Inspector

    public TextMeshProUGUI starsText; // Assign the UI Text element for stars display in the Inspector
    public TextMeshProUGUI coinText;

    public string sceneToLoad = "GamePlay"; // Adjust this to your game scene's name

    void Start()
    {
        playButton.onClick.AddListener(() => LoadScene(sceneToLoad));
        startButton.onClick.AddListener(() => LoadScene(sceneToLoad));
        skinsButton.onClick.AddListener(() => TogglePanel(skinsPanel, true));
        storeButton.onClick.AddListener(() => TogglePanel(storePanel, true));
        skinsCloseButton.onClick.AddListener(() => TogglePanel(skinsPanel, false));
        storeCloseButton.onClick.AddListener(() => TogglePanel(storePanel, false));

        // Update the stars display using PlayerProgress instead of PlayerPrefs
        UpdateStarsDisplay();
        UpdateCoins();
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void TogglePanel(GameObject panel, bool show)
    {
        if (panel != null)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogError("No CanvasGroup found on the panel GameObject.");
                return;
            }

            if (show)
            {
                panel.SetActive(true);
                panel.transform.localScale = Vector3.one * 0.95f; // Start slightly smaller
                panel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack); // Scale animation
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, 0.3f); // Fade in
            }
            else
            {
                panel.transform.DOScale(0.95f, 0.3f).SetEase(Ease.InBack)
                    .OnComplete(() => panel.SetActive(false));
                canvasGroup.DOFade(0, 0.3f); // Fade out
            }
        }
        else
        {
            Debug.LogError("Panel GameObject is not assigned.");
        }
    }

    // This method will load the saved stars count from PlayerProgress and update the UI
    void UpdateStarsDisplay()
    {
        if (PlayerProgress.Instance != null)
        {
            int totalStars = PlayerProgress.Instance.GetTotalStars();
            if (starsText != null)
            {
                //starsText.text = $"Total Stars: {totalStars}";
                starsText.text = $"{totalStars}";
                Debug.Log("Stars displayed in UI: " + totalStars);
            }
            else
            {
                Debug.LogError("StarsText is not assigned in the Inspector!");
            }
        }
        else
        {
            Debug.LogError("PlayerProgress instance is null!");
        }
    }
    public void UpdateCoins()
    {
        Debug.Log("Updating coins");
        coinText.text = PlayerPrefs.GetInt("Coins").ToString();
    }

}
