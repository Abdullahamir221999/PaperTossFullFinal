using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public TextMeshProUGUI starCountText; // Reference to the UI Text for stars

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        UpdateButtonState();
        button.onClick.AddListener(OnClick);
    }

    public void UpdateButtonState()
    {
        bool isUnlocked = PlayerProgress.Instance.IsLevelUnlocked(levelIndex);
        button.interactable = isUnlocked;

        // Update the star count for this level
        int starsForLevel = PlayerProgress.Instance.GetStarsForLevel(levelIndex);

        // Set the star count in the text UI
        if (starCountText != null)
        {
            starCountText.text = starsForLevel.ToString();
        }
    }

    private void OnClick()
    {
        LevelManager.Instance.LoadLevel(levelIndex);
    }
}
