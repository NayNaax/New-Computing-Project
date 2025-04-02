using UnityEngine;
using System.Collections; // Needed for Coroutine

// Modified TimedSequenceManager.cs
// Using TimedSequenceManager.cs [cite: uploaded:Assets/Scripts/Platforms/TimedSequenceManager.cs]
public class TimedSequenceManager : MonoBehaviour
{
    [Header("Sequence Settings")]
    [Tooltip("Duration of the timer started by Plate 1")]
    public float sequenceTimerDuration = 10f; // Set your desired timer (e.g., 10 seconds)
    [Tooltip("Animator component for the door(s)")]
    public Animator doorAnimator; // Assign your door's Animator here
    [Tooltip("Name of the boolean parameter in the Animator to open the door")]
    public string doorOpenParameter = "IsOpen";

    // --- Internal State ---
    private bool isTimerRunning = false;
    private bool plate1Activated = false; // Tracks if plate 1 started the sequence
    private bool plate2ActivatedSuccessfully = false; // Tracks if plate 2 was hit in time
    private Coroutine timerCoroutine = null;

    // --- Public Methods Called by Plates ---

    // Called by Plate 1's Trigger Script
    public void StartPlate1Sequence()
    {
        // Only start if the timer isn't already running
        if (!isTimerRunning)
        {
            Debug.Log($"PLATE 1 TRIGGERED: Starting {sequenceTimerDuration} second timer NOW!");

            isTimerRunning = true;
            plate1Activated = true;
            plate2ActivatedSuccessfully = false; // Reset success flag

            // Stop any previous timer just in case
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            // Start the countdown coroutine that WILL close the door eventually
            timerCoroutine = StartCoroutine(DoorControlTimer());
        }
        else
        {
             Debug.Log("Plate 1 Activated, but timer was already running.");
        }
    }

    // Called by Plate 2's Trigger Script
    public void ActivatePlate2()
    {
        // Check if the timer was started by plate 1 and is still running
        if (isTimerRunning && plate1Activated)
        {
            Debug.Log("Plate 2 Activated within time limit!");

            // Mark as successful, so the timer expiration knows not to log failure
            plate2ActivatedSuccessfully = true;

            // Open the door immediately
            OpenDoor();

            // *** CRITICAL CHANGE: Do NOT stop the timerCoroutine here ***
            // The timer will continue and eventually call CloseDoor()
        }
        else if (!isTimerRunning && plate1Activated)
        {
             Debug.Log("Plate 2 Activated, but timer had already expired or sequence was completed.");
        }
        else
        {
             Debug.Log("Plate 2 Activated, but Plate 1 sequence was not active.");
        }
    }

    // --- Timer and Door Logic ---

    // This coroutine now ONLY handles the end-of-timer state: closing the door.
    private IEnumerator DoorControlTimer()
    {
        yield return new WaitForSeconds(sequenceTimerDuration);

        // Timer has finished.
        Debug.Log("Sequence Timer Expired! Closing door.");

        CloseDoor(); // Close the door regardless of success state

        // Reset state variables
        isTimerRunning = false;
        plate1Activated = false;
        plate2ActivatedSuccessfully = false;
        timerCoroutine = null;
        // Add any other reset logic needed (e.g., visual feedback)
    }

    private void OpenDoor()
    {
        if (doorAnimator != null)
        {
            Debug.Log($"Opening door (setting {doorOpenParameter} to true).");
            // Only open if it's not already open, or handle as needed
            if (!doorAnimator.GetBool(doorOpenParameter))
            {
                doorAnimator.SetBool(doorOpenParameter, true);
            }
        }
        else
        {
            Debug.LogWarning("Door Animator not assigned to TimedSequenceManager!");
        }
    }

    private void CloseDoor()
    {
        if (doorAnimator != null)
        {
             Debug.Log($"Closing door (setting {doorOpenParameter} to false).");
             // Only close if it's currently open
             if (doorAnimator.GetBool(doorOpenParameter))
             {
                doorAnimator.SetBool(doorOpenParameter, false);
             }
        }
         else
        {
            Debug.LogWarning("Door Animator not assigned to TimedSequenceManager!");
        }
    }

    // Optional: You might not need a separate ResetSequence public method anymore,
    // but keeping it ensures the initial state is correct.
    public void ResetSequence()
    {
        Debug.Log("Resetting sequence state.");
        isTimerRunning = false;
        plate1Activated = false;
        plate2ActivatedSuccessfully = false;

         if (timerCoroutine != null)
         {
             StopCoroutine(timerCoroutine);
             timerCoroutine = null;
         }

        CloseDoor(); // Ensure door is closed initially
    }

     void Start()
     {
         // Ensure the sequence starts in a reset state with the door closed.
         ResetSequence();
     }
}