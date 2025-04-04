using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlateTrigger : MonoBehaviour
{
    [Tooltip("Assign the TimedSequenceManager GameObject here.")]
    public TimedSequenceManager sequenceManager;

    [Tooltip("Set to 1 for the starting plate, 2 for the second plate.")]
    public int plateID = 0; // 1 or 2

    private void Start()
    {
        // Basic validation
        if (sequenceManager == null)
        {
            Debug.LogError($"Sequence Manager not assigned on Pressure Plate '{gameObject.name}'!", this);
        }
        if (plateID != 1 && plateID != 2)
        {
            Debug.LogError($"Invalid Plate ID ({plateID}) assigned on Pressure Plate '{gameObject.name}'. Must be 1 or 2.", this);
        }

        // Ensure the collider is a trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
             Debug.LogWarning($"Collider on Pressure Plate '{gameObject.name}' is not set to 'Is Trigger'. Automatically setting it.", this);
             col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Something entered trigger on {gameObject.name}: {other.gameObject.name} with tag {other.tag}");
        
        if (sequenceManager != null && other.CompareTag("Player"))
        {
            Debug.Log($"Player entered Plate {plateID} ({gameObject.name})");

            // Trigger appropriate action based on plate ID
            if (plateID == 1)
            {
                sequenceManager.StartPlate1Sequence();
            }
            else if (plateID == 2)
            {
                sequenceManager.ActivatePlate2();
            }
        }
        else if (sequenceManager == null)
        {
             // Log only once to avoid spam
             if (!loggedManagerNullError)
             {
                  Debug.LogError($"Sequence Manager is null on Plate {plateID} ({gameObject.name}) trigger script.", this);
                  loggedManagerNullError = true;
             }
        }
    }

     private bool loggedManagerNullError = false;
}