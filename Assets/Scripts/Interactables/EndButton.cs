using UnityEngine;
using UnityEngine.SceneManagement;

public class EndButton : Interactable
{
    [Tooltip("Name of the scene to load when the button is pressed.")]
    public string nextSceneName;

    public override void Interact()
    {
        base.Interact(); // Invoke events from the base class

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set on EndButton on " + gameObject.name);
        }
    }

    private void Reset()
    {
        promptMessage = "Next Level";
    }
}