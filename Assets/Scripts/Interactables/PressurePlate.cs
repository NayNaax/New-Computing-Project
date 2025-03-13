using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Tooltip("Check if the activator needs the PerspectiveObject script.")]
    public bool requiresPerspectiveObject = true;

    // We no longer need activatorTag

    [Tooltip("Event triggered when the plate is activated.")]
    public UnityEvent onPlateActivated;

    [Tooltip("Event triggered when the plate is deactivated.")]
    public UnityEvent onPlateDeactivated;

    private int objectsOnPlate = 0;

    // Add this to reference the EndButton
    private GameObject endButton;

    void Start()
    {
        // Find the EndButton in the scene
        endButton = GameObject.FindGameObjectWithTag("EndButton");

        // Ensure the EndButton is found
        if (endButton == null)
        {
            Debug.LogError("EndButton not found. Make sure it has the 'EndButton' tag.");
        }

        // Initially hide the EndButton
        if (endButton != null)
        {
            endButton.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has the PerspectiveObject component
        if (requiresPerspectiveObject)
        {
            if (other.GetComponent<PerspectiveObject>() != null)
            {
                objectsOnPlate++;
                if (objectsOnPlate == 1)
                {
                    onPlateActivated?.Invoke();
                    Debug.Log("Pressure Plate Activated!");
                    // Show the EndButton when the plate is activated
                    if (endButton != null)
                    {
                        endButton.SetActive(true);
                    }
                }
            }
        }
        else
        {
            objectsOnPlate++;
            if (objectsOnPlate == 1)
            {
                onPlateActivated?.Invoke();
                Debug.Log("Pressure Plate Activated!");
                // Show the EndButton when the plate is activated
                if (endButton != null)
                {
                    endButton.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object has the PerspectiveObject component
        if (requiresPerspectiveObject)
        {
            if (other.GetComponent<PerspectiveObject>() != null)
            {
                objectsOnPlate--;
                if (objectsOnPlate == 0)
                {
                    onPlateDeactivated?.Invoke();
                    Debug.Log("Pressure Plate Deactivated!");
                    // Hide the EndButton when the plate is deactivated
                    if (endButton != null)
                    {
                        endButton.SetActive(false);
                    }
                }
            }
        }
        else
        {
            objectsOnPlate--;
            if (objectsOnPlate == 0)
            {
                onPlateDeactivated?.Invoke();
                Debug.Log("Pressure Plate Deactivated!");
                // Hide the EndButton when the plate is deactivated
                if (endButton != null)
                {
                    endButton.SetActive(false);
                }
            }
        }
    }
}