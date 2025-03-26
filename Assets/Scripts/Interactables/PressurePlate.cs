using UnityEngine;
// Removed using UnityEngine.Events; as we are replacing UnityEvents

public class PressurePlate : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("Check if the activator needs the PerspectiveObject script.")]
    public bool requiresPerspectiveObject = true;

    [Header("Door Control Settings")]
    [Tooltip("Drag the Animator component of the first door here.")]
    public Animator door1Animator;
    [Tooltip("Drag the Animator component of the second door here.")]
    public Animator door2Animator; // Added for the second door
    [Tooltip("The exact name of the BOOLEAN parameter in the door Animators (e.g., IsOpen).")]
    public string animationParameterName = "IsOpen";

    // --- Removed UnityEvents ---
    // public UnityEvent onPlateActivated;
    // public UnityEvent onPlateDeactivated;
    // --- Removed EndButton logic ---
    // private GameObject endButton;

    private int objectsOnPlate = 0;

    // --- Removed Start method (related to EndButton) ---
    // void Start() { ... }

    private void OnTriggerEnter(Collider other)
    {
        bool shouldActivate = false;
        if (requiresPerspectiveObject)
        {
            // Check if the colliding object has the PerspectiveObject component
            if (other.GetComponent<PerspectiveObject>() != null)
            {
                shouldActivate = true;
                // --- ADDED Debug Log ---
                Debug.Log($"OnTriggerEnter: Valid object '{other.name}' entered.");
                // -----------------------
            }
            // --- ADDED Debug Log ---
            else
            {
                 Debug.Log($"OnTriggerEnter: Object '{other.name}' entered but lacks PerspectiveObject script.");
            }
            // -----------------------
        }
        else // If any object can activate it
        {
             shouldActivate = true;
             // --- ADDED Debug Log ---
             Debug.Log($"OnTriggerEnter: Object '{other.name}' entered (any object allowed).");
             // -----------------------
        }

        if (shouldActivate)
        {
            objectsOnPlate++;
            // --- ADDED Debug Log ---
            Debug.Log($"OnTriggerEnter: objectsOnPlate count incremented to: {objectsOnPlate}");
            // -----------------------
            // Only trigger on the first object entering
            if (objectsOnPlate == 1)
            {
                Debug.Log("Pressure Plate Activated! (First object)");
                // --- Call SetBool directly ---
                SetDoorStates(true); // Call helper function to open doors
                // --- Removed onPlateActivated?.Invoke(); ---
                // --- Removed endButton.SetActive(true); ---
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
         bool shouldDeactivate = false;
         if (requiresPerspectiveObject)
         {
             // Check if the colliding object has the PerspectiveObject component
             if (other.GetComponent<PerspectiveObject>() != null)
             {
                 shouldDeactivate = true;
                 // --- ADDED Debug Log ---
                 Debug.Log($"OnTriggerExit: Valid object '{other.name}' exited.");
                 // -----------------------
             }
             // --- ADDED Debug Log ---
             else
            {
                 Debug.Log($"OnTriggerExit: Object '{other.name}' exited but lacks PerspectiveObject script.");
            }
            // -----------------------
         }
         else // If any object can deactivate it
         {
              shouldDeactivate = true;
              // --- ADDED Debug Log ---
              Debug.Log($"OnTriggerExit: Object '{other.name}' exited (any object allowed).");
              // -----------------------
         }

         if (shouldDeactivate)
         {
             // Ensure count doesn't go below zero
             if (objectsOnPlate > 0)
             {
                 objectsOnPlate--;
                 // --- ADDED Debug Log ---
                 Debug.Log($"OnTriggerExit: objectsOnPlate count decremented to: {objectsOnPlate}");
                 // -----------------------
                 // Only trigger when the last valid object leaves
                 if (objectsOnPlate == 0)
                 {
                     Debug.Log("Pressure Plate Deactivated! (Last object)");
                     // --- Call SetBool directly ---
                     SetDoorStates(false); // Call helper function to close doors
                     // --- Removed onPlateDeactivated?.Invoke(); ---
                     // --- Removed endButton.SetActive(false); ---
                 }
             }
             // --- ADDED Debug Log ---
             else
             {
                 Debug.LogWarning($"OnTriggerExit: objectsOnPlate was already 0 for object '{other.name}'.");
             }
             // -----------------------
         }
    }

    // Helper function to set the state for both doors
    private void SetDoorStates(bool openState)
    {
        // --- ADDED Debug Log ---
        Debug.Log($"SetDoorStates called with: {openState} at time: {Time.time}");
        // -----------------------

        if (string.IsNullOrEmpty(animationParameterName))
        {
            Debug.LogError("Animation Parameter Name is not set on Pressure Plate!", this);
            return;
        }

        if (door1Animator != null)
        {
            // --- ADDED Debug Log ---
            Debug.Log($"Setting '{animationParameterName}' to {openState} on Door 1 Animator: {door1Animator.gameObject.name}");
            // -----------------------
            door1Animator.SetBool(animationParameterName, openState);
        }
        else
        {
             Debug.LogWarning("Door 1 Animator not assigned to Pressure Plate!", this);
        }

        if (door2Animator != null)
        {
             // --- ADDED Debug Log ---
             Debug.Log($"Setting '{animationParameterName}' to {openState} on Door 2 Animator: {door2Animator.gameObject.name}");
             // -----------------------
            door2Animator.SetBool(animationParameterName, openState);
        }
         else
        {
             // Optional: Uncomment if you expect Door 2 to always be assigned
             // Debug.LogWarning("Door 2 Animator not assigned to Pressure Plate!", this);
        }
    }
}