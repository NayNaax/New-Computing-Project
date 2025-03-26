using UnityEngine;
using System.Collections;

// Using ShootableTarget.cs [cite: uploaded:Assets/Scripts/Interactables/ShootableTarget.cs]
public class ShootableTarget : MonoBehaviour
{
    [Tooltip("The GameObject (e.g., pressure plate 2) hidden by this target.")]
    public GameObject hidingObject;

    [Tooltip("Should the hiding object be destroyed (won't reappear) or just disabled?")]
    public bool destroyHidingObject = false;

    [Header("Block Reset Settings")] // Added Header
    [Tooltip("Drag the specific movable block that should reset its position here.")]
    public GameObject movableBlockToReset; // Reference to the block

    // --- Timer State Variables ---
    private float timerRemaining = 0f;
    private bool isTimerActive = false;
    private Coroutine timerCoroutine = null;

    // --- Block Position Variables ---
    private Vector3 initialBlockPosition; // To store the block's starting position
    private bool blockPositionStored = false; // Flag to ensure position is stored only once

    // --- Optional: UI for Timer Display ---
    // public UnityEngine.UI.Text timerDisplay;

    void Start()
    {
        // Ensure the hiding object is visible and timer is off when the game starts
        ResetTargetState(); // This already handles hidingObject visibility

        // Store the initial position of the movable block if assigned
        if (movableBlockToReset != null)
        {
            initialBlockPosition = movableBlockToReset.transform.position;
            blockPositionStored = true;
            Debug.Log($"Stored initial position for {movableBlockToReset.name}: {initialBlockPosition}");
        }
        else
        {
            Debug.LogWarning($"Movable Block To Reset is not assigned on target '{gameObject.name}'. Block position will not be reset.", this);
        }
    }

    // Update remains the same (optional UI logic)
    void Update()
    {
        // --- Optional: Update Timer UI ---
        // if (timerDisplay != null)
        // {
        //     if (isTimerActive)
        //     {
        //         timerDisplay.text = $"Time: {timerRemaining:F1}";
        //     }
        //     else
        //     {
        //         timerDisplay.text = "";
        //     }
        // }
        // ---------------------------------
    }

    // Activate method remains the same
    public void Activate(float duration)
    {
        if (hidingObject == null && !destroyHidingObject)
        {
            Debug.LogWarning($"Hiding object not assigned to target '{gameObject.name}'. Cannot activate timer.", this);
            return;
        }

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            Debug.Log("Previous timer stopped.");
        }

        Debug.Log($"Target activated! Revealing pressure plate for {duration} seconds.", this);

        if (!destroyHidingObject)
        {
            if (hidingObject != null) hidingObject.SetActive(false);
        }
        else
        {
            if (hidingObject != null)
            {
                Debug.LogWarning("Hiding object destroyed, it will not reappear.", this);
                Destroy(hidingObject);
            }
            return;
        }

        timerCoroutine = StartCoroutine(StartTimer(duration));
    }

    // Coroutine to handle the countdown
    private IEnumerator StartTimer(float duration)
    {
        timerRemaining = duration;
        isTimerActive = true;
        Debug.Log($"Timer started: {duration}s");

        while (timerRemaining > 0f)
        {
            timerRemaining -= Time.deltaTime;
            yield return null;
        }

        // Timer finished
        timerRemaining = 0f;
        isTimerActive = false;
        timerCoroutine = null;
        Debug.Log("Timer finished!");

        // --- START BLOCK RESET LOGIC ---
        // Re-enable the hiding object (pressure plate 2)
        if (!destroyHidingObject && hidingObject != null)
        {
            Debug.Log("Re-hiding pressure plate.");
            hidingObject.SetActive(true);
        }

        // Reset the movable block's position if it's assigned and position was stored
        if (movableBlockToReset != null && blockPositionStored)
        {
            Debug.Log($"Timer ended. Resetting position of {movableBlockToReset.name} to {initialBlockPosition}");

            // Stop physics momentum before teleporting (optional but good practice)
            Rigidbody blockRb = movableBlockToReset.GetComponent<Rigidbody>();
            if (blockRb != null)
            {
                blockRb.velocity = Vector3.zero;
                blockRb.angularVelocity = Vector3.zero;
            }

            // Directly set the position
            movableBlockToReset.transform.position = initialBlockPosition;

            // If using PerspectiveObjectManager, you might need to update its internal target point too,
            // although teleporting should generally be okay. If issues arise, this might need refinement.
        }
        // --- END BLOCK RESET LOGIC ---
    }

    // ResetTargetState remains largely the same, just ensures hidingObject is active
    public void ResetTargetState()
    {
         if (timerCoroutine != null)
         {
             StopCoroutine(timerCoroutine);
             timerCoroutine = null;
         }
         isTimerActive = false;
         timerRemaining = 0f;

         if (!destroyHidingObject && hidingObject != null)
         {
             hidingObject.SetActive(true);
         }
         // --- Optional: Clear Timer UI ---
         // if (timerDisplay != null) timerDisplay.text = "";
         // ---------------------------------
         Debug.Log("Target state reset (ensured hiding object is visible).");
         // Note: This does NOT reset the movable block position, only the timer/plate visibility.
         // Block reset only happens when the timer naturally runs out.
    }

    void OnDestroy()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }
}