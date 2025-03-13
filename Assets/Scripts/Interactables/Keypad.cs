using UnityEngine;
using System.Collections; // Required for Coroutines

#if UNITY_EDITOR
using UnityEditor; // Required for Editor-specific functions
#endif

public class Keypad : Interactable
{
    [Tooltip("The door GameObject to be controlled.")]
    public GameObject door;

    [Tooltip("Animator used for the door.")]
    public Animator doorAnimator;

    [Tooltip("Name of the boolean parameter in the Animator to control the door state.")]
    public string animationParameterName = "IsOpen";

    [Tooltip("Initial open state of the door.")]
    public bool doorOpen = false;

    private bool initialized = false; // Flag to track initialization

    private void Start()
    {
        // Only initialize in Play mode
        if (Application.isPlaying)
        {
            InitializeKeypad();
        }
    }

    // Add OnEnable for Editor initialization
    #if UNITY_EDITOR
    private void OnEnable()
    {
        // Only try to initialize in Edit mode through OnEnable
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall += SafeInitialize;
        }
    }

    private void SafeInitialize()
    {
        // Check if this object still exists
        if (this != null && !initialized)
        {
            InitializeKeypad();
        }
    }
    #endif

    private void InitializeKeypad()
    {
        if (initialized) return; // Prevent double initialization

        // Check if door is assigned
        if (door == null)
        {
            // Instead of Debug.LogError which clutters the console
            // Just silently disable the component in Edit mode
            enabled = false;
            return;
        }

        // Check for Animator component
        if (doorAnimator == null)
        {
            doorAnimator = door.GetComponent<Animator>();
            if (doorAnimator == null)
            {
                // Again, silently disable instead of logging
                enabled = false;
                return;
            }
        }

        // Set initial door state
        if (Application.isPlaying) // Only set animator values during play mode
        {
            doorAnimator.SetBool(animationParameterName, doorOpen);
        }

        initialized = true;
    }

    public override void Interact()
    {
        if (!initialized)
        {
            InitializeKeypad(); // Ensure initialization
            if (!initialized) return; // If initialization failed, exit
        }

        base.Interact(); // Call the base Interact to invoke the event

        doorOpen = !doorOpen;
        doorAnimator.SetBool(animationParameterName, doorOpen);
        //Debug.Log("Keypad Interact");
    }

    private void Reset()
    {
        promptMessage = "Use Keypad";

        // Try to automatically assign door and animator
        if (door == null)
        {
            door = GameObject.FindGameObjectWithTag("Door"); // You might need to tag your door with "Door"
        }

        if (door != null && doorAnimator == null)
        {
            doorAnimator = door.GetComponent<Animator>();
        }
    }
}