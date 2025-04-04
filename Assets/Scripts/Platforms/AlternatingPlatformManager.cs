using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Needed for using Lists

public class AlternatingPlatformManager : MonoBehaviour
{
    // Define a structure to hold a pair of platforms and their components
    [System.Serializable] // Makes this visible in the Inspector
    public class PlatformPair
    {
        public string pairName = "Platform Pair"; // Optional name for Inspector clarity
        public GameObject platformA;
        public GameObject platformB;

        // Internal references (no need to set in Inspector)
        [HideInInspector] public Renderer rendererA;
        [HideInInspector] public Collider colliderA;
        [HideInInspector] public Renderer rendererB;
        [HideInInspector] public Collider colliderB;
        [HideInInspector] public bool isAVisible = true; // Track state per pair
    }

    [Header("Platform Pairs")]
    [Tooltip("Add all pairs of platforms that should alternate here.")]
    public List<PlatformPair> platformPairs = new List<PlatformPair>();

    [Header("Global Timing Settings")]
    [Tooltip("Minimum time (in seconds) before platforms switch state.")]
    public float minTime = 1.0f;
    [Tooltip("Maximum time (in seconds) before platforms switch state.")]
    public float maxTime = 3.0f;

    private List<Coroutine> runningCoroutines = new List<Coroutine>();

    void Start()
    {
        runningCoroutines.Clear();

        if (platformPairs.Count == 0)
        {
             Debug.LogWarning("AlternatingPlatformManager: No platform pairs assigned in the list.", this);
             return;
        }

        // Initialize each pair
        foreach (PlatformPair pair in platformPairs)
        {
            if (!ValidateAndInitializePair(pair))
            {
                // Stop initialization if a pair is invalid to avoid errors
                Debug.LogError($"Initialization failed for pair '{pair.pairName}'. Aborting.", this);
                // Disable the manager if any pair fails setup
                 this.enabled = false;
                 // Make sure any started coroutines are stopped
                 StopAllRunningCoroutines();
                return;
            }

            // Set initial state (A visible, B invisible by default)
            pair.isAVisible = true; // Or randomize: Random.value > 0.5f;
            SetPlatformState(pair.platformA, pair.rendererA, pair.colliderA, pair.isAVisible);
            SetPlatformState(pair.platformB, pair.rendererB, pair.colliderB, !pair.isAVisible);

            // Start a separate alternating cycle for each pair
            Coroutine routine = StartCoroutine(AlternationCycle(pair));
            runningCoroutines.Add(routine);
        }
    }

    bool ValidateAndInitializePair(PlatformPair pair)
    {
        if (pair.platformA == null || pair.platformB == null)
        {
            Debug.LogError($"AlternatingPlatformManager: Pair '{pair.pairName}' has unassigned platform(s)!", this);
            return false;
        }

        // Get components for Platform A
        pair.rendererA = pair.platformA.GetComponent<Renderer>();
        pair.colliderA = pair.platformA.GetComponent<Collider>();
        if (pair.rendererA == null || pair.colliderA == null)
        {
            Debug.LogError($"AlternatingPlatformManager: Platform A ({pair.platformA.name}) in pair '{pair.pairName}' is missing a Renderer or Collider!", pair.platformA);
            return false;
        }

        // Get components for Platform B
        pair.rendererB = pair.platformB.GetComponent<Renderer>();
        pair.colliderB = pair.platformB.GetComponent<Collider>();
         if (pair.rendererB == null || pair.colliderB == null)
        {
            Debug.LogError($"AlternatingPlatformManager: Platform B ({pair.platformB.name}) in pair '{pair.pairName}' is missing a Renderer or Collider!", pair.platformB);
            return false;
        }
        return true; // Pair is valid
    }

    // Coroutine now takes the specific pair it manages
    private IEnumerator AlternationCycle(PlatformPair pair)
    {
        // Ensure pair and components are valid before starting the loop
        if (pair == null || pair.platformA == null || pair.platformB == null ||
            pair.rendererA == null || pair.colliderA == null ||
            pair.rendererB == null || pair.colliderB == null)
        {
             Debug.LogError($"AlternationCycle cannot start for pair '{pair?.pairName}' due to missing components or references.", this);
             yield break; // Stop this specific coroutine
        }

        while (true)
        {
            // Wait for a random duration
            float switchDelay = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(switchDelay);

             // Check again inside the loop in case objects were destroyed
             if (pair.platformA == null || pair.platformB == null)
             {
                 Debug.LogWarning($"A platform in pair '{pair.pairName}' was destroyed. Stopping alternation for this pair.", this);
                 yield break; // Exit coroutine for this pair
             }

            // Toggle the state for this specific pair
            pair.isAVisible = !pair.isAVisible;

            // Apply the new state to this pair's platforms
            SetPlatformState(pair.platformA, pair.rendererA, pair.colliderA, pair.isAVisible);
            SetPlatformState(pair.platformB, pair.rendererB, pair.colliderB, !pair.isAVisible);
        }
    }

    // Helper function to enable/disable a platform's components
    private void SetPlatformState(GameObject platform, Renderer rend, Collider col, bool isVisible)
    {
         // Check if components still exist before trying to enable/disable them
        if (rend != null) rend.enabled = isVisible;
        if (col != null) col.enabled = isVisible;
    }

    private void StopAllRunningCoroutines()
    {
         foreach (var routine in runningCoroutines)
         {
             if (routine != null)
             {
                 StopCoroutine(routine);
             }
         }
         runningCoroutines.Clear();
         Debug.Log("Stopped all platform alternation coroutines.");
    }

     // Optional: Clean up coroutines if the manager is disabled/destroyed
    void OnDisable()
    {
        StopAllRunningCoroutines();
        // Optionally reset all platforms to a default visible state
        // foreach(PlatformPair pair in platformPairs) {
        //     if (pair != null && pair.platformA != null && pair.platformB != null) {
        //         SetPlatformState(pair.platformA, pair.rendererA, pair.colliderA, true);
        //         SetPlatformState(pair.platformB, pair.rendererB, pair.colliderB, true);
        //     }
        // }
    }
}