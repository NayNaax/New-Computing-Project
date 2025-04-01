using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Debug.Log("Resume button clicked.");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor when resuming
        Cursor.visible = false; // Hide the cursor when resuming
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor when paused
        Cursor.visible = true; // Make the cursor visible when paused
    }

    void Start()
    {
        // Ensure the pause menu is hidden when the level starts
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        // Keep GameIsPaused as false, as set in the declaration
        GameIsPaused = false;
        // Ensure timescale is normal at start
        Time.timeScale = 1f;
        // Ensure cursor is locked/hidden at start (matching PlayerLook)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart()
    {
        Debug.Log("Restart button clicked.");
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked. Exiting application...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the Editor
        #else
            Application.Quit(); // Quit the application in a build
        #endif
    }
}