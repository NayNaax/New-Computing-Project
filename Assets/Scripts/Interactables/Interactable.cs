using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events; // Important for events

public class Interactable : MonoBehaviour
{
    [Tooltip("Message displayed to the player when in range.")]
    public string promptMessage;

    [Tooltip("Event triggered when the object is interacted with.")]
    public UnityEvent onInteract; // Using a UnityEvent

    // Interaction logic
    public virtual void Interact()
    {
        onInteract?.Invoke(); // Safely invoke the event
        //Debug.Log($"Interacted with {gameObject.name}"); // Helpful for debugging
    }

    private void Reset()
    {
        // Add a default prompt message if none is provided
        if (string.IsNullOrEmpty(promptMessage))
        {
            promptMessage = "Interact";
        }
    }
}