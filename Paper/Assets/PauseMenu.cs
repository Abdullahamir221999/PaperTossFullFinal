using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign this to your Pause Menu panel in the Inspector

    // Pauses the game
    public void PauseGame()
    {
        Time.timeScale = 0f; // Pauses the game
        pauseMenuUI.SetActive(true); // Show the pause menu UI
    }

    // Resumes the game
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resumes the game
        pauseMenuUI.SetActive(false); // Hide the pause menu UI
    }
}