/*using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    public int stars = 0;
    public int currentLevel = 1;

    public int[] starsRequiredForAreas = { 10, 20 }; // Example thresholds for unlocking areas

    private void Awake()
    {
        Debug.Log("PlayerProgress Awake called");
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerProgress instance created and set to DontDestroyOnLoad");
        }
        else
        {
            Debug.Log("Duplicate PlayerProgress instance destroyed");
            Destroy(gameObject);
        }
    }

    public void AddStar()
    {
        stars++;
        CheckAreaUnlock();
    }

    void CheckAreaUnlock()
    {
        for (int i = 0; i < starsRequiredForAreas.Length; i++)
        {
            if (stars >= starsRequiredForAreas[i])
            {
                UnlockArea(i + 1);
            }
        }
    }

    void UnlockArea(int areaIndex)
    {
        Debug.Log("Area " + areaIndex + " unlocked!");
        // Add logic to unlock the area in your game, e.g., enabling levels in the next area
    }

    public bool IsAreaUnlocked(int areaIndex)
    {
        return stars >= starsRequiredForAreas[areaIndex - 1];
    }
}
*/
/*using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public int stars = 0;
    public int currentLevel = 1;
    public int maxUnlockedLevel = 1;
    public int[] starsRequiredForAreas = { 0, 10, 20, 30, 40 }; // Adjust as needed

    private void Awake()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        DontDestroyOnLoad(gameObject);
        Debug.Log("PlayerProgress Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerProgress instance created and set to DontDestroyOnLoad");
        }
        else if (Instance != this)
        {
            Debug.Log("Duplicate PlayerProgress instance destroyed");
            Destroy(gameObject);
        }
    }

    public bool IsLevelUnlocked(int level)
    {
        bool result = level <= maxUnlockedLevel;
        Debug.Log($"Checking if level {level} is unlocked. Result: {result}");
        return result;
    }

    public bool IsAreaUnlocked(int areaIndex)
    {
        if (areaIndex <= 0 || areaIndex > starsRequiredForAreas.Length)
        {
            return false;
        }
        return stars >= starsRequiredForAreas[areaIndex - 1];
    }

    public void AddStar()
    {
        stars++;
        CheckAreaUnlock();
    }

    void CheckAreaUnlock()
    {
        for (int i = 1; i < starsRequiredForAreas.Length; i++)
        {
            if (stars >= starsRequiredForAreas[i])
            {
                UnlockArea(i + 1);
            }
        }
    }

    void UnlockArea(int areaIndex)
    {
        Debug.Log($"Area {areaIndex} unlocked!");
    }

    public void UnlockNextLevel()
    {
        maxUnlockedLevel = Mathf.Min(maxUnlockedLevel + 1, 10); // Assuming 10 is the max level
        Debug.Log($"Next level unlocked. Max unlocked level: {maxUnlockedLevel}");
    }
}*/
/*using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public int stars = 0;
    public int currentLevel = 1;
    public int maxUnlockedLevel = 1;
    public int[] starsRequiredForAreas = { 0, 10, 20, 30, 40 }; // Adjust as needed

    private void Awake()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved progress
            LoadProgress();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void LoadProgress()
    {
        stars = PlayerPrefs.GetInt("Stars", 0);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);
    }

    public bool IsLevelUnlocked(int level)
    {
        return level <= maxUnlockedLevel;
    }

    public bool IsAreaUnlocked(int areaIndex)
    {
        if (areaIndex <= 0 || areaIndex > starsRequiredForAreas.Length)
        {
            return false;
        }
        return stars >= starsRequiredForAreas[areaIndex - 1];
    }

    public void AddStar()
    {
        stars++;
        PlayerPrefs.SetInt("Stars", stars); // Save stars to PlayerPrefs
        PlayerPrefs.Save();
        CheckAreaUnlock();
    }

    void CheckAreaUnlock()
    {
        for (int i = 1; i < starsRequiredForAreas.Length; i++)
        {
            if (stars >= starsRequiredForAreas[i])
            {
                UnlockArea(i + 1);
            }
        }
    }

    void UnlockArea(int areaIndex)
    {
        Debug.Log($"Area {areaIndex} unlocked!");
    }

    public void UnlockNextLevel()
    {
        maxUnlockedLevel = Mathf.Min(maxUnlockedLevel + 1, 10); // Assuming 10 is the max level
        PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel); // Save level progress to PlayerPrefs
        PlayerPrefs.Save();
        Debug.Log($"Next level unlocked. Max unlocked level: {maxUnlockedLevel}");
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);
        PlayerPrefs.Save();
    }
}
*/
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    public int currentLevel = 1;
    public int maxUnlockedLevel = 1;

    private const int maxStarsPerLevel = 5;
    public int[] starsForLevels; // Array to store stars for each level
    public int totalStars = 0; // Total stars earned across all levels

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
            Debug.Log("PlayerProgress instance created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate PlayerProgress instance destroyed.");
        }
    }

    private void LoadProgress()
    {
        int levels = 10; // Assuming you have 10 levels
        starsForLevels = new int[levels];

        for (int i = 0; i < levels; i++)
        {
            starsForLevels[i] = PlayerPrefs.GetInt($"StarsForLevel{i + 1}", 0);
            Debug.Log($"Loaded stars for level {i + 1}: {starsForLevels[i]}");
        }

        totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);
        Debug.Log("Progress loaded. Total Stars: " + totalStars);
    }

    public void UpdateStarsForLevel(int level, int starsEarned)
    {
        if (level < 1 || level > starsForLevels.Length) return;

        // Ensure the player doesn't earn more than the max stars for the level
        starsEarned = Mathf.Clamp(starsEarned, 0, maxStarsPerLevel);

        if (starsEarned > starsForLevels[level - 1])
        {
            // Update total stars by adding the improvement
            int starDifference = starsEarned - starsForLevels[level - 1];
            totalStars += starDifference;
            starsForLevels[level - 1] = starsEarned;

            // Save the progress
            PlayerPrefs.SetInt($"StarsForLevel{level}", starsForLevels[level - 1]);
            PlayerPrefs.SetInt("TotalStars", totalStars);
            PlayerPrefs.Save();
        }
    }

    public int GetStarsForLevel(int level)
    {
        if (level < 1 || level > starsForLevels.Length) return 0;
        return starsForLevels[level - 1];
    }

    public int GetTotalStars()
    {
        return totalStars;
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);

        for (int i = 0; i < starsForLevels.Length; i++)
        {
            PlayerPrefs.SetInt($"StarsForLevel{i + 1}", starsForLevels[i]);
            Debug.Log($"Saving stars for level {i + 1}: {starsForLevels[i]}");
        }

        PlayerPrefs.SetInt("TotalStars", totalStars);
        PlayerPrefs.Save();
        Debug.Log("Progress saved. Total Stars: " + totalStars);
    }


    public bool IsLevelUnlocked(int level)
    {
        return level <= maxUnlockedLevel;
    }

    // Add this method to unlock the next level
    public void UnlockNextLevel()
    {
        if (currentLevel >= maxUnlockedLevel)
        {
            maxUnlockedLevel = currentLevel + 1;
            PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);
            PlayerPrefs.Save();
            Debug.Log($"Next level unlocked! Max Unlocked Level: {maxUnlockedLevel}");
        }
    }
}

