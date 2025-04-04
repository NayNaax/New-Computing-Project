using UnityEngine;
using UnityEngine.UI; // Required if you interact with Button component

// Attach this script to the Button GameObject used for quitting after Level 3 boss
[RequireComponent(typeof(Button))] // Ensure a Button component exists
public class EndButtonQuit : MonoBehaviour
{
    private Button quitButton;

    // Called when the script instance is first loaded
    void Awake()
    {
        quitButton = GetComponent<Button>();
        if (quitButton == null)
        {
            Debug.LogError("EndButtonQuit script needs a Button component on the same GameObject.", this);
            return;
        }

        // Add listener for the button click
        quitButton.onClick.AddListener(QuitApplication);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide the button initially when the scene loads
        gameObject.SetActive(false);
        Debug.Log("EndButtonQuit hidden on start.");
    }

    // Public method to make the button visible (called by the boss script)
    public void ShowButton()
    {
        gameObject.SetActive(true);
        Debug.Log("EndButtonQuit is now visible.");
    }

    // Method called when the button is clicked
    public void QuitApplication()
    {
        Debug.Log("Quit Application button clicked.");

        // Quit logic (handles Editor vs. Build)
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the Editor
        #else
            Application.Quit(); // Quit the built application
        #endif
    }

    // Optional: Ensure listener is removed when object is destroyed
    void OnDestroy()
    {
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitApplication);
        }
    }
}