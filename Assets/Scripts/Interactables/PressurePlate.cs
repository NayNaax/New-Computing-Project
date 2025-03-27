using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("Check if the activator needs the PerspectiveObject script.")]
    public bool requiresPerspectiveObject = true;

    [Header("Door Control Settings")]
    [Tooltip("Drag the Animator component of the first door here.")]
    public Animator door1Animator;
    [Tooltip("Drag the Animator component of the second door here.")]
    public Animator door2Animator;
    [Tooltip("The exact name of the BOOLEAN parameter in the door Animators (e.g., IsOpen).")]
    public string animationParameterName = "IsOpen";

    private int objectsOnPlate = 0;

    private void OnTriggerEnter(Collider other)
    {
        bool shouldActivate = false;
        if (requiresPerspectiveObject)
        {
            // Check if the colliding object has the PerspectiveObject component
            if (other.GetComponent<PerspectiveObject>() != null)
            {
                shouldActivate = true;
                Debug.Log($"OnTriggerEnter: Valid object '{other.name}' entered.");
            }
            else
            {
                Debug.Log($"OnTriggerEnter: Object '{other.name}' entered but lacks PerspectiveObject script.");
            }
        }
        else // If any object can activate it
        {
            shouldActivate = true;
            Debug.Log($"OnTriggerEnter: Object '{other.name}' entered (any object allowed).");
        }

        if (shouldActivate)
        {
            objectsOnPlate++;
            Debug.Log($"OnTriggerEnter: objectsOnPlate count incremented to: {objectsOnPlate}");
            
            // Only trigger on the first object entering
            if (objectsOnPlate == 1)
            {
                Debug.Log("Pressure Plate Activated! (First object)");
                SetDoorStates(true); // Call helper function to open doors
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
                 Debug.Log($"OnTriggerExit: Valid object '{other.name}' exited.");
             }
             else
             {
                 Debug.Log($"OnTriggerExit: Object '{other.name}' exited but lacks PerspectiveObject script.");
             }
         }
         else // If any object can deactivate it
         {
              shouldDeactivate = true;
              Debug.Log($"OnTriggerExit: Object '{other.name}' exited (any object allowed).");
         }

         if (shouldDeactivate)
         {
             // Ensure count doesn't go below zero
             if (objectsOnPlate > 0)
             {
                 objectsOnPlate--;
                 Debug.Log($"OnTriggerExit: objectsOnPlate count decremented to: {objectsOnPlate}");
                 
                 // Only trigger when the last valid object leaves
                 if (objectsOnPlate == 0)
                 {
                     Debug.Log("Pressure Plate Deactivated! (Last object)");
                     SetDoorStates(false); // Call helper function to close doors
                 }
             }
             else
             {
                 Debug.LogWarning($"OnTriggerExit: objectsOnPlate was already 0 for object '{other.name}'.");
             }
         }
    }

    // Helper function to set the state for both doors
    private void SetDoorStates(bool openState)
    {
        Debug.Log($"SetDoorStates called with: {openState} at time: {Time.time}");

        if (string.IsNullOrEmpty(animationParameterName))
        {
            Debug.LogError("Animation Parameter Name is not set on Pressure Plate!", this);
            return;
        }

        if (door1Animator != null)
        {
            Debug.Log($"Setting '{animationParameterName}' to {openState} on Door 1 Animator: {door1Animator.gameObject.name}");
            door1Animator.SetBool(animationParameterName, openState);
        }
        else
        {
             Debug.LogWarning("Door 1 Animator not assigned to Pressure Plate!", this);
        }

        if (door2Animator != null)
        {
             Debug.Log($"Setting '{animationParameterName}' to {openState} on Door 2 Animator: {door2Animator.gameObject.name}");
            door2Animator.SetBool(animationParameterName, openState);
        }
    }
}