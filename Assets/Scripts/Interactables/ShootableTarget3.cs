using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required for using Lists

// New script: ShootableTarget3.cs
// Place this in your Assets/Scripts/Interactables folder
public class ShootableTarget3 : MonoBehaviour, IShootable // Implement the IShootable interface
{
    [Header("Wall Control Settings")]
    [Tooltip("Drag the GameObjects (blocks for the THIRD wall/barrier) to hide/disable here.")]
    public List<GameObject> wallBlocks3 = new List<GameObject>(); // List for the third set of blocks

    [Tooltip("Optional: Tag to check for the bullseye hit on this target, e.g., TargetYellow3")]
    public string bullseyeTag = "TargetYellow"; // You might use the same or different tags

    [Header("Reward Durations (Seconds)")]
    [Tooltip("Duration the third wall stays hidden for a bullseye hit.")]
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

    // Called by GUN.cs when this target is hit
    public void ActivateReward(string hitTag)
    {
        float duration = 0f;

        if (hitTag == bullseyeTag)
        {
            duration = bullseyeDuration;
            Debug.Log($"ShootableTarget3: Bullseye hit! Hiding wall 3 for {duration}s.");
        }
        else if (hitTag == "TargetRed")
        {
            duration = outerRing1Duration;
            Debug.Log($"ShootableTarget3: Outer Ring 1 hit! Hiding wall 3 for {duration}s.");
        }
        else if (hitTag == "TargetBlue")
        {
            duration = outerRing2Duration;
            Debug.Log($"ShootableTarget3: Outer Ring 2 hit! Hiding wall 3 for {duration}s.");
        }
        else if (hitTag == "TargetWhite")
        {
            duration = outerRing3Duration;
            Debug.Log($"ShootableTarget3: Outer Ring 3 hit! Hiding wall 3 for {duration}s.");
        }
        else
        {
            Debug.LogWarning($"ShootableTarget3: Received unknown hitTag '{hitTag}'. No action taken.", this);
            return;
        }

        if (wallCoroutine != null)
        {
            StopCoroutine(wallCoroutine);
            Debug.Log("ShootableTarget3: Stopping existing wall timer.");
        }

        wallCoroutine = StartCoroutine(WallControlCoroutine(duration));
    }

    private IEnumerator WallControlCoroutine(float duration)
    {
        Debug.Log($"ShootableTarget3: Hiding wall blocks 3 for {duration} seconds.");
        SetWallState(false);
        isWallHidden = true;

        yield return new WaitForSeconds(duration);

        Debug.Log("ShootableTarget3: Timer finished! Showing wall blocks 3.");
        SetWallState(true);
        isWallHidden = false;
        wallCoroutine = null;
    }

    private void SetWallState(bool activeState)
    {
        if (wallBlocks3 == null || wallBlocks3.Count == 0)
        {
            Debug.LogWarning("ShootableTarget3: No blocks assigned to 'Wall Blocks 3' list!", this);
            return;
        }

        foreach (GameObject block in wallBlocks3)
        {
            if (block != null)
            {
                block.SetActive(activeState);
            }
            else
            {
                Debug.LogWarning("ShootableTarget3: A null GameObject was found in the 'Wall Blocks 3' list!", this);
            }
        }
        Debug.Log($"ShootableTarget3: Set wall blocks 3 active state to: {activeState}");
    }

    void OnDestroy()
    {
        if (wallCoroutine != null)
        {
            StopCoroutine(wallCoroutine);
        }
    }
}