using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlternatingPlatformManager : MonoBehaviour
{
    // Define a structure to hold a pair of platforms and their components
    [System.Serializable]
    public class PlatformPair
    {
        public string pairName = "Platform Pair"; // Optional name for Inspector clarity
        public GameObject platformA;
        public GameObject platformB;

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

        foreach (PlatformPair pair in platformPairs)
        {
            if (!ValidateAndInitializePair(pair))
            {
                Debug.LogError($"Initialization failed for pair '{pair.pairName}'. Aborting.", this);
                this.enabled = false;
                StopAllRunningCoroutines();
                return;
            }

            // Set initial state (A visible, B invisible by default)
            pair.isAVisible = true;
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
        return true;
    }

    private IEnumerator AlternationCycle(PlatformPair pair)
    {
        // Ensure pair and components are valid before starting the loop
        if (pair == null || pair.platformA == null || pair.platformB == null ||
            pair.rendererA == null || pair.colliderA == null ||
            pair.rendererB == null || pair.colliderB == null)
        {
             Debug.LogError($"AlternationCycle cannot start for pair '{pair?.pairName}' due to missing components or references.", this);
             yield break;
        }

        while (true)
        {
            float switchDelay = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(switchDelay);

             // Check in case objects were destroyed during runtime
             if (pair.platformA == null || pair.platformB == null)
             {
                 Debug.LogWarning($"A platform in pair '{pair.pairName}' was destroyed. Stopping alternation for this pair.", this);
                 yield break;
             }

            // Toggle the state for this specific pair
            pair.isAVisible = !pair.isAVisible;

            // Apply the new state to this pair's platforms
            SetPlatformState(pair.platformA, pair.rendererA, pair.colliderA, pair.isAVisible);
            SetPlatformState(pair.platformB, pair.rendererB, pair.colliderB, !pair.isAVisible);
        }
    }

    private void SetPlatformState(GameObject platform, Renderer rend, Collider col, bool isVisible)
    {
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

    void OnDisable()
    {
        StopAllRunningCoroutines();
    }
}