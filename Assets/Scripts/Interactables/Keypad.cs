using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        // Ensure the Animator component is assigned
        // if (door == null)
        // {
        //     Debug.LogError("Door GameObject not assigned to Keypad script on " + gameObject.name);
        //     enabled = false; // Disable the script to prevent errors
        //     return;
        // }

        // if (doorAnimator == null)
        // {
        //     doorAnimator = door.GetComponent<Animator>();
        //     if (doorAnimator == null)
        //     {
        //         Debug.LogError("No Animator component found on the door GameObject assigned to Keypad script on " + gameObject.name);
        //         enabled = false;
        //         return;
        //     }
        // }

        // Set the initial state of the door
        // doorAnimator.SetBool(animationParameterName, doorOpen);
    }

    public override void Interact()
    {
        base.Interact(); // Call the base Interact to invoke the event

        doorOpen = !doorOpen;
        doorAnimator.SetBool(animationParameterName, doorOpen);
    }

    private void Reset()
    {
        promptMessage = "Use Keypad";

        //Try to automatically assign door and animator
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