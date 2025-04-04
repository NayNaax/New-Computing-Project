using UnityEngine;
using System.Collections;

public class TimedButtonDoor : Interactable
{
    [Header("Door Settings")]
    [Tooltip("Assign the Animator component of the door(s) here.")]
    public Animator doorAnimator;

    [Tooltip("The exact name of the BOOLEAN parameter in the door Animator (e.g., IsOpen).")]
    public string animationParameterName = "IsOpen";

    [Tooltip("How long the door stays open (in seconds).")]
    public float openDuration = 3.0f;

    private Coroutine closeDoorCoroutine = null;
    private bool isDoorCurrentlyOpen = false; // Track door state managed by this button

    void Start()
    {
        // Basic validation
        if (doorAnimator == null)
        {
            Debug.LogError($"Door Animator is not assigned on TimedButtonDoor script on '{gameObject.name}'!", this);
            enabled = false; // Disable script if setup is incomplete
            return;
        }

        // Ensure the door starts closed according to this script's logic
        SetDoorState(false);
    }

    // This method is called by PlayerInteract when the player interacts with this button
    public override void Interact()
    {
        base.Interact(); // Call base class event if needed

        if (doorAnimator == null) return; // Extra safety check

        Debug.Log($"Button '{gameObject.name}' interacted with.");

        // If a close timer was already running, stop it.
        if (closeDoorCoroutine != null)
        {
            StopCoroutine(closeDoorCoroutine);
            Debug.Log("Stopped previous close timer.");
        }

        SetDoorState(true);
        closeDoorCoroutine = StartCoroutine(CloseDoorAfterDelay());
    }

    private IEnumerator CloseDoorAfterDelay()
    {
        Debug.Log($"Door will close in {openDuration} seconds.");
        yield return new WaitForSeconds(openDuration);

        Debug.Log("Timer finished. Closing door.");
        SetDoorState(false);
        closeDoorCoroutine = null; // Clear the coroutine reference
    }

    private void SetDoorState(bool shouldBeOpen)
    {
        if (doorAnimator != null)
        {
            // Check if the parameter exists before trying to set it
            bool parameterExists = false;
            foreach (AnimatorControllerParameter param in doorAnimator.parameters)
            {
                if (param.name == animationParameterName && param.type == AnimatorControllerParameterType.Bool)
                {
                    parameterExists = true;
                    break;
                }
            }

            if (parameterExists)
            {
                doorAnimator.SetBool(animationParameterName, shouldBeOpen);
                isDoorCurrentlyOpen = shouldBeOpen;
            }
            else
            {
                 Debug.LogError($"Animator on '{doorAnimator.gameObject.name}' does not have a BOOLEAN parameter named '{animationParameterName}'.", this);
            }
        }
    }

    // Called when the script component is reset in the Inspector
    private void Reset()
    {
        promptMessage = "Press Button";
    }

    void OnDisable()
    {
        if (closeDoorCoroutine != null)
        {
            StopCoroutine(closeDoorCoroutine);
            closeDoorCoroutine = null;
        }
    }
}