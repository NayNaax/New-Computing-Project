using UnityEngine;
using System.Collections;

// Modified ShootableTarget.cs
// Using ShootableTarget.cs [cite: uploaded:Assets/Scripts/Interactables/ShootableTarget.cs]
public class ShootableTarget : MonoBehaviour, IShootable // Implement the IShootable interface
{
    [Tooltip("The GameObject (e.g., pressure plate 2) hidden by this target.")]
    public GameObject hidingObject;

    [Tooltip("Should the hiding object be destroyed (won't reappear) or just disabled?")]
    public bool destroyHidingObject = false;

    [Header("Block Reset Settings")]
    [Tooltip("Drag the specific movable block that should reset its position here.")]
    public GameObject movableBlockToReset;

    [Header("Reward Durations (Seconds)")] // Added durations here too for consistency
    [Tooltip("Duration the hidingObject stays hidden for a bullseye hit.")]
    public float bullseyeDuration = 12f; // Keep original script's default if desired
    [Tooltip("Duration for hitting the next outer ring (e.g., Red).")]
    public float outerRing1Duration = 8f;
    [Tooltip("Duration for hitting the middle ring (e.g., Blue).")]
    public float outerRing2Duration = 5f;
    [Tooltip("Duration for hitting the outermost ring (e.g., White).")]
    public float outerRing3Duration = 3f;

    // --- Timer State Variables ---
    private float timerRemaining = 0f;
    private bool isTimerActive = false;
    private Coroutine timerCoroutine = null;

    // --- Block Position Variables ---
    private Vector3 initialBlockPosition;
    private bool blockPositionStored = false;

    // --- Optional: UI for Timer Display ---
    // public UnityEngine.UI.Text timerDisplay;

    void Start()
    {
        ResetTargetState();

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

    void Update()
    {
        // Optional UI update logic remains the same
    }

    // --- MODIFIED: Renamed Activate to ActivateReward and added hitTag parameter ---
    public void ActivateReward(string hitTag)
    {
         if (hidingObject == null && !destroyHidingObject)
        {
            Debug.LogWarning($"Hiding object not assigned to target '{gameObject.name}'. Cannot activate timer.", this);
            return;
        }

        float duration = 0f;

        // Determine duration based on the hit tag
        if (hitTag == "TargetYellow") // Assuming this target uses same tags
        {
            duration = bullseyeDuration;
        }
        else if (hitTag == "TargetRed")
        {
            duration = outerRing1Duration;
        }
         else if (hitTag == "TargetBlue")
        {
            duration = outerRing2Duration;
        }
         else if (hitTag == "TargetWhite")
        {
            duration = outerRing3Duration;
        }
        else
        {
             Debug.LogWarning($"ShootableTarget (Original): Received unknown hitTag '{hitTag}'. No action taken.", this);
             return;
        }

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            Debug.Log("Previous timer stopped.");
        }

        Debug.Log($"Target activated! Revealing hiding object for {duration} seconds.", this);

        if (!destroyHidingObject)
        {
            if (hidingObject != null) hidingObject.SetActive(false);
        }
        else
        {
             // Destroy logic remains the same
             if (hidingObject != null)
             {
                 Debug.LogWarning("Hiding object destroyed, it will not reappear.", this);
                 Destroy(hidingObject);
             }
             return; // Don't start timer if destroyed
        }

        timerCoroutine = StartCoroutine(StartTimer(duration));
    }
    // --- END MODIFICATION ---


    // Coroutine to handle the countdown (logic remains the same)
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

        // Re-enable the hiding object
        if (!destroyHidingObject && hidingObject != null)
        {
            Debug.Log("Re-hiding object.");
            hidingObject.SetActive(true);
        }

        // Reset the movable block's position
        if (movableBlockToReset != null && blockPositionStored)
        {
            Debug.Log($"Timer ended. Resetting position of {movableBlockToReset.name} to {initialBlockPosition}");
            Rigidbody blockRb = movableBlockToReset.GetComponent<Rigidbody>();
            if (blockRb != null)
            {
                blockRb.velocity = Vector3.zero;
                blockRb.angularVelocity = Vector3.zero;
            }
            movableBlockToReset.transform.position = initialBlockPosition;
        }
    }

    // ResetTargetState logic remains the same
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
         Debug.Log("Target state reset.");
    }

    // OnDestroy logic remains the same
    void OnDestroy()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }
}