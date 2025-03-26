using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required for using Lists

// New script: shootabletarget2.cs
// Place this in your Assets/Scripts/Interactables folder
public class ShootableTarget2 : MonoBehaviour, IShootable // Implement the IShootable interface
{
    [Header("Wall Control Settings")]
    [Tooltip("Drag the GameObjects (blocks forming the wall) to hide/disable here.")]
    public List<GameObject> wallBlocks = new List<GameObject>(); // List of blocks to control

    [Tooltip("Optional: Tag to check for the bullseye hit, e.g., TargetYellow")]
    public string bullseyeTag = "TargetYellow"; // Assuming TargetYellow is bullseye based on GUN.cs

    [Header("Reward Durations (Seconds)")]
    [Tooltip("Duration the wall stays hidden for a bullseye hit.")]
    public float bullseyeDuration = 10f; // As requested
    [Tooltip("Duration for hitting the next outer ring (e.g., Red).")]
    public float outerRing1Duration = 7f; // Example duration
    [Tooltip("Duration for hitting the middle ring (e.g., Blue).")]
    public float outerRing2Duration = 5f; // Example duration
    [Tooltip("Duration for hitting the outermost ring (e.g., White).")]
    public float outerRing3Duration = 3f; // Example duration

    // --- Timer State Variables ---
    private Coroutine wallCoroutine = null;
    private bool isWallHidden = false;

    void Start()
    {
        // Ensure wall is visible at the start
        SetWallState(true);
        isWallHidden = false;
    }

    // This method will be called by GUN.cs when this target is hit
    // It implements the IShootable interface method
    public void ActivateReward(string hitTag)
    {
        float duration = 0f;

        // Determine duration based on the tag passed from GUN.cs
        // Using placeholder tags consistent with original GUN.cs
        if (hitTag == bullseyeTag) // e.g., "TargetYellow"
        {
            duration = bullseyeDuration;
            Debug.Log($"ShootableTarget2: Bullseye hit! Hiding wall for {duration}s.");
        }
        else if (hitTag == "TargetRed") // Example outer ring 1
        {
            duration = outerRing1Duration;
             Debug.Log($"ShootableTarget2: Outer Ring 1 hit! Hiding wall for {duration}s.");
        }
        else if (hitTag == "TargetBlue") // Example outer ring 2
        {
             duration = outerRing2Duration;
             Debug.Log($"ShootableTarget2: Outer Ring 2 hit! Hiding wall for {duration}s.");
        }
        else if (hitTag == "TargetWhite") // Example outer ring 3
        {
             duration = outerRing3Duration;
             Debug.Log($"ShootableTarget2: Outer Ring 3 hit! Hiding wall for {duration}s.");
        }
        else
        {
            Debug.LogWarning($"ShootableTarget2: Received unknown hitTag '{hitTag}'. No action taken.", this);
            return; // Do nothing if the tag is not recognized
        }

        // Stop any existing timer coroutine before starting a new one
        if (wallCoroutine != null)
        {
            StopCoroutine(wallCoroutine);
            Debug.Log("ShootableTarget2: Stopping existing wall timer.");
        }

        // Start the coroutine to hide the wall
        wallCoroutine = StartCoroutine(WallControlCoroutine(duration));
    }

    // Coroutine to hide the wall blocks and then show them again
    private IEnumerator WallControlCoroutine(float duration)
    {
        Debug.Log($"ShootableTarget2: Hiding wall blocks for {duration} seconds.");
        SetWallState(false); // Hide the wall
        isWallHidden = true;

        yield return new WaitForSeconds(duration); // Wait for the specified time

        Debug.Log("ShootableTarget2: Timer finished! Showing wall blocks.");
        SetWallState(true); // Show the wall again
        isWallHidden = false;
        wallCoroutine = null; // Clear the coroutine reference
    }

    // Helper function to set the active state for all wall blocks
    private void SetWallState(bool activeState)
    {
        if (wallBlocks == null || wallBlocks.Count == 0)
        {
             Debug.LogWarning("ShootableTarget2: No blocks assigned to 'Wall Blocks' list!", this);
             return;
        }

        foreach (GameObject block in wallBlocks)
        {
            if (block != null)
            {
                block.SetActive(activeState);
            }
             else
            {
                 Debug.LogWarning("ShootableTarget2: A null GameObject was found in the 'Wall Blocks' list!", this);
            }
        }
        Debug.Log($"ShootableTarget2: Set wall blocks active state to: {activeState}");
    }

    // Ensure coroutine stops if the target is destroyed
    void OnDestroy()
    {
        if (wallCoroutine != null)
        {
            StopCoroutine(wallCoroutine);
        }
    }
}

// Define the interface in a separate file or here for simplicity
public interface IShootable
{
    // Method to activate the target's specific reward
    // Takes the tag of the hit part to determine the reward level
    void ActivateReward(string hitTag);
}