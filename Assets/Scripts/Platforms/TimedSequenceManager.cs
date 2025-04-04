using UnityEngine;
using System.Collections;

public class TimedSequenceManager : MonoBehaviour
{
    [Header("Sequence Settings")]
    [Tooltip("Duration of the timer started by Plate 1")]
    public float sequenceTimerDuration = 10f;
    [Tooltip("Animator component for the door(s)")]
    public Animator doorAnimator;
    [Tooltip("Name of the boolean parameter in the Animator to open the door")]
    public string doorOpenParameter = "IsOpen";

    // --- Internal State ---
    private bool isTimerRunning = false;
    private bool plate1Activated = false;
    private bool plate2ActivatedSuccessfully = false;
    private Coroutine timerCoroutine = null;

    // --- Public Methods Called by Plates ---

    // Called by Plate 1's Trigger Script
    public void StartPlate1Sequence()
    {
        if (!isTimerRunning)
        {
            Debug.Log($"PLATE 1 TRIGGERED: Starting {sequenceTimerDuration} second timer NOW!");

            isTimerRunning = true;
            plate1Activated = true;
            plate2ActivatedSuccessfully = false;

            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
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
        if (isTimerRunning && plate1Activated)
        {
            Debug.Log("Plate 2 Activated within time limit!");
            plate2ActivatedSuccessfully = true;
            OpenDoor();

            // *** CRITICAL: Do NOT stop the timerCoroutine here ***
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

    // This coroutine handles the end-of-timer state: closing the door
    private IEnumerator DoorControlTimer()
    {
        yield return new WaitForSeconds(sequenceTimerDuration);

        Debug.Log("Sequence Timer Expired! Closing door.");
        CloseDoor();

        isTimerRunning = false;
        plate1Activated = false;
        plate2ActivatedSuccessfully = false;
        timerCoroutine = null;
    }

    private void OpenDoor()
    {
        if (doorAnimator != null)
        {
            Debug.Log($"Opening door (setting {doorOpenParameter} to true).");
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

        CloseDoor();
    }

     void Start()
     {
         ResetSequence();
     }
}