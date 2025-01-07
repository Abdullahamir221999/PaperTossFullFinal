using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private GameObject levelsParent;

    private void Awake()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        DontDestroyOnLoad(gameObject);
        Debug.Log("LevelManager Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("LevelManager instance created and set to DontDestroyOnLoad");
        }
        else if (Instance != this)
        {
            Debug.Log("Duplicate LevelManager instance destroyed");
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded called. Scene name: {scene.name}");
        if (scene.name == "GamePlay")
        {
            Debug.Log("GamePlay scene detected, attempting to find LevelsParent");
            levelsParent = GameObject.Find("LevelsParent");
            if (levelsParent == null)
            {
                Debug.LogError("LevelsParent GameObject not found in the GamePlay scene!");
                Debug.Log("Root objects in the scene:");
                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    Debug.Log(obj.name);
                }
                return;
            }
            Debug.Log($"LevelsParent found: {levelsParent.name}");
            ActivateCurrentLevel();
        }
    }

    public void LoadLevel(int levelIndex)
    {
        
        PlayerPrefs.SetInt("ChallengeMode", 0);
        PlayerPrefs.SetInt("UnlimitedMode", 0);
        
        Debug.Log($"LoadLevel called with levelIndex: {levelIndex}");
        PlayerProgress.Instance.currentLevel = levelIndex;
        Debug.Log($"Loading GamePlay scene. Current level set to: {PlayerProgress.Instance.currentLevel}");
        SceneManager.LoadScene("GamePlay");
    }
    public void LoadNextLevel()
    {
        int nextLevel = PlayerProgress.Instance.currentLevel + 1;
        Debug.Log($"LoadNextLevel called. Next level: {nextLevel}");
        if (nextLevel <= PlayerProgress.Instance.maxUnlockedLevel)
        {
            LoadLevel(nextLevel);
        }
        else
        {
            Debug.Log("No more levels available. Returning to MainMenu");
            SceneManager.LoadScene("MainMenu");
        }
    }
    private void ActivateCurrentLevel()
    {
        Debug.Log($"ActivateCurrentLevel called. Current level: {PlayerProgress.Instance.currentLevel}");
        if (levelsParent == null)
        {
            Debug.LogError("levelsParent is null in ActivateCurrentLevel!");
            return;
        }

        foreach (Transform level in levelsParent.transform)
        {
            level.gameObject.SetActive(false);
            Debug.Log($"Deactivated level: {level.name}");
        }

        string currentLevelName = "Level" + PlayerProgress.Instance.currentLevel;
        Debug.Log($"Searching for level: {currentLevelName}");
        Transform currentLevel = levelsParent.transform.Find(currentLevelName);
        if (currentLevel != null)
        {
            currentLevel.gameObject.SetActive(true);
            Debug.Log($"Activated level: {currentLevel.name}");
        }
        else
        {
            Debug.LogError($"Level {currentLevelName} not found!");
            Debug.Log("Children of LevelsParent:");
            foreach (Transform child in levelsParent.transform)
            {
                Debug.Log(child.name);
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("LevelManager OnDestroy called");
        if (Instance == this)
        {
            Debug.LogError("LevelManager instance is being destroyed unexpectedly!");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
