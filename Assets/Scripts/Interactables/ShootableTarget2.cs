using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootableTarget2 : MonoBehaviour, IShootable
{
    [Header("Wall Control Settings")]
    [Tooltip("Drag the GameObjects (blocks forming the wall) to hide/disable here.")]
    public List<GameObject> wallBlocks = new List<GameObject>();

    [Tooltip("Optional: Tag to check for the bullseye hit, e.g., TargetYellow")]
    public string bullseyeTag = "TargetYellow"; // Assuming TargetYellow is bullseye based on GUN.cs

    [Header("Reward Durations (Seconds)")]
    [Tooltip("Duration the wall stays hidden for a bullseye hit.")]
    public float bullseyeDuration = 10f;
    [Tooltip("Duration for hitting the next outer ring (e.g., Red).")]
    public float outerRing1Duration = 7f;
    [Tooltip("Duration for hitting the middle ring (e.g., Blue).")]
    public float outerRing2Duration = 5f;
    [Tooltip("Duration for hitting the outermost ring (e.g., White).")]
    public float outerRing3Duration = 3f;

    private Coroutine wallCoroutine = null;
    private bool isWallHidden = false;

    void Start()
    {
        SetWallState(true);
        isWallHidden = false;
    }

    // Implements the IShootable interface method - called by GUN.cs when target is hit
    public void ActivateReward(string hitTag)
    {
        float duration = 0f;

        // Determine duration based on the tag passed from GUN.cs
        if (hitTag == bullseyeTag)
        {
            duration = bullseyeDuration;
            Debug.Log($"ShootableTarget2: Bullseye hit! Hiding wall for {duration}s.");
        }
        else if (hitTag == "TargetRed")
        {
            duration = outerRing1Duration;
            Debug.Log($"ShootableTarget2: Outer Ring 1 hit! Hiding wall for {duration}s.");
        }
        else if (hitTag == "TargetBlue")
        {
            duration = outerRing2Duration;
            Debug.Log($"ShootableTarget2: Outer Ring 2 hit! Hiding wall for {duration}s.");
        }
        else if (hitTag == "TargetWhite")
        {
            duration = outerRing3Duration;
            Debug.Log($"ShootableTarget2: Outer Ring 3 hit! Hiding wall for {duration}s.");
        }
        else
        {
            Debug.LogWarning($"ShootableTarget2: Received unknown hitTag '{hitTag}'. No action taken.", this);
            return;
        }

        // Stop any existing timer coroutine before starting a new one
        if (wallCoroutine != null)
        {
            StopCoroutine(wallCoroutine);
            Debug.Log("ShootableTarget2: Stopping existing wall timer.");
        }

        wallCoroutine = StartCoroutine(WallControlCoroutine(duration));
    }

    private IEnumerator WallControlCoroutine(float duration)
    {
        Debug.Log($"ShootableTarget2: Hiding wall blocks for {duration} seconds.");
        SetWallState(false);
        isWallHidden = true;

        yield return new WaitForSeconds(duration);

        Debug.Log("ShootableTarget2: Timer finished! Showing wall blocks.");
        SetWallState(true);
        isWallHidden = false;
        wallCoroutine = null;
    }

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

    void OnDestroy()
    {
        if (wallCoroutine != null)
        {
            StopCoroutine(wallCoroutine);
        }
    }
}

// Interface definition for target shooting interactions
public interface IShootable
{
    void ActivateReward(string hitTag);
}