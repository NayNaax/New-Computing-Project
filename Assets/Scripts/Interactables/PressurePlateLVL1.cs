using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Needed to check the current scene

public class PressurePlateLVL1 : MonoBehaviour
{
    [Header("Level Specific Settings")]
    [Tooltip("The scene index for Level 1.")]
    public int level1SceneIndex = 1; // Set this to the build index of the Level 1 scene

    [Header("Button Control Settings")]
    [Tooltip("Drag the EndButton GameObject here.")]
    public GameObject endButton;

    [Header("Activation Settings")]
    [Tooltip("Tag required for the activating object (e.g., CubeA, CubeB). Leave empty if only PerspectiveObject script is needed.")]
    public string requiredTag = "";

    private int objectsOnPlate = 0;
    private bool isLevel1 = false;

    void Start()
    {
        isLevel1 = SceneManager.GetActiveScene().buildIndex == level1SceneIndex;

        if (!isLevel1)
        {
             Debug.LogWarning($"PressurePlateLVL1 script is active in scene index {SceneManager.GetActiveScene().buildIndex}, but configured for index {level1SceneIndex}. It will not function here.");
             // Optionally disable the component if not in the correct level
             return;
        }

        if (endButton == null)
        {
            Debug.LogError("EndButton is not assigned in the inspector for PressurePlateLVL1!", this);
            this.enabled = false;
            return;
        }

        endButton.SetActive(false);
        objectsOnPlate = 0;
        Debug.Log("PressurePlateLVL1 Initialized for Level 1. EndButton hidden.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLevel1) return;

        PerspectiveObject perspectiveObj = other.GetComponent<PerspectiveObject>();

        if (perspectiveObj != null)
        {
            if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
            {
                objectsOnPlate++;
                Debug.Log($"Valid object '{other.name}' entered Plate LVL1. Count: {objectsOnPlate}");

                // Show the button if this is the first valid object
                if (objectsOnPlate == 1)
                {
                    endButton.SetActive(true);
                    Debug.Log("EndButton Activated (First Object on Plate LVL1).");
                }
            }
            else
            {
                Debug.Log($"Object '{other.name}' has PerspectiveObject but incorrect tag ('{other.tag}'). Plate LVL1 ignored.");
            }
        }
        else
        {
            Debug.Log($"Object '{other.name}' lacks PerspectiveObject script. Plate LVL1 ignored.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isLevel1) return;

        PerspectiveObject perspectiveObj = other.GetComponent<PerspectiveObject>();

        if (perspectiveObj != null)
        {
            if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
            {
                // Ensure count doesn't go below zero
                if (objectsOnPlate > 0)
                {
                    objectsOnPlate--;
                    Debug.Log($"Valid object '{other.name}' exited Plate LVL1. Count: {objectsOnPlate}");

                    // Hide the button if this was the last valid object
                    if (objectsOnPlate == 0)
                    {
                        endButton.SetActive(false);
                        Debug.Log("EndButton Deactivated (Last Object left Plate LVL1).");
                    }
                }
                else
                {
                    Debug.LogWarning($"OnTriggerExit: objectsOnPlate was already 0 for object '{other.name}' on Plate LVL1.");
                }
            }
        }
    }
}