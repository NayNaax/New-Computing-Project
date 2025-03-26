using UnityEngine;
using System.Collections.Generic; // Required for Lists

// New script: PressurePlate2.cs
public class PressurePlate2 : MonoBehaviour // Changed class name
{
    [Header("Activation Settings")]
    [Tooltip("Check if the activator needs the PerspectiveObject script.")]
    public bool requiresPerspectiveObject = true;

    [Header("Controlled Wall Blocks")]
    [Tooltip("Drag the GameObjects (blocks forming the wall) to hide/disable here.")]
    public List<GameObject> wallBlocks = new List<GameObject>(); // Renamed list for clarity

    private int objectsOnPlate = 0;

    private void OnTriggerEnter(Collider other)
    {
        bool shouldActivate = false;
        if (requiresPerspectiveObject)
        {
            // Check if the colliding object has the PerspectiveObject component
            // Assuming PerspectiveObject.cs is located at 'Assets/Scripts/Perspective Objects/PerspectiveObject.cs' [cite: uploaded:Assets/Scripts/Perspective Objects/PerspectiveObject.cs]
            if (other.GetComponent<PerspectiveObject>() != null)
            {
                shouldActivate = true;
                Debug.Log($"OnTriggerEnter: Valid object '{other.name}' entered Pressure Plate 2.");
            }
            else
            {
                 Debug.Log($"OnTriggerEnter: Object '{other.name}' entered Pressure Plate 2 but lacks PerspectiveObject script.");
            }
        }
        else // If any object can activate it
        {
             shouldActivate = true;
             Debug.Log($"OnTriggerEnter: Object '{other.name}' entered Pressure Plate 2 (any object allowed).");
        }

        if (shouldActivate)
        {
            objectsOnPlate++;
            Debug.Log($"OnTriggerEnter (Pressure Plate 2): objectsOnPlate count incremented to: {objectsOnPlate}");

            // Only trigger on the first object entering
            if (objectsOnPlate == 1)
            {
                Debug.Log("Pressure Plate 2 Activated! Hiding wall.");
                SetWallState(false); // Disable wall blocks
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
         bool shouldDeactivate = false;
         if (requiresPerspectiveObject)
         {
             // Check if the exiting object has the PerspectiveObject component
             // Assuming PerspectiveObject.cs is located at 'Assets/Scripts/Perspective Objects/PerspectiveObject.cs' [cite: uploaded:Assets/Scripts/Perspective Objects/PerspectiveObject.cs]
             if (other.GetComponent<PerspectiveObject>() != null)
             {
                 shouldDeactivate = true;
                 Debug.Log($"OnTriggerExit: Valid object '{other.name}' exited Pressure Plate 2.");
             }
              else
             {
                  Debug.Log($"OnTriggerExit: Object '{other.name}' exited Pressure Plate 2 but lacks PerspectiveObject script.");
             }
         }
         else // If any object can deactivate it
         {
              shouldDeactivate = true;
              Debug.Log($"OnTriggerExit: Object '{other.name}' exited Pressure Plate 2 (any object allowed).");
         }

         if (shouldDeactivate)
         {
             // Ensure count doesn't go below zero
             if (objectsOnPlate > 0)
             {
                 objectsOnPlate--;
                 Debug.Log($"OnTriggerExit (Pressure Plate 2): objectsOnPlate count decremented to: {objectsOnPlate}");

                 // Only trigger when the last valid object leaves
                 if (objectsOnPlate == 0)
                 {
                     Debug.Log("Pressure Plate 2 Deactivated! (Last object). Showing wall.");
                     SetWallState(true); // Re-enable wall blocks immediately
                 }
             }
             else
             {
                 Debug.LogWarning($"OnTriggerExit (Pressure Plate 2): objectsOnPlate was already 0 for object '{other.name}'.");
             }
         }
    }

    // Helper function to set the active state for all wall blocks in the list
    private void SetWallState(bool activeState)
    {
        Debug.Log($"SetWallState (Pressure Plate 2) called with: {activeState} at time: {Time.time}");

        if (wallBlocks == null || wallBlocks.Count == 0)
        {
             Debug.LogWarning("No blocks assigned to 'Wall Blocks' list on Pressure Plate 2!", this);
             return;
        }

        foreach (GameObject block in wallBlocks)
        {
            if (block != null)
            {
                Debug.Log($"Setting GameObject '{block.name}' active state to: {activeState}");
                block.SetActive(activeState);
            }
            else
            {
                 Debug.LogWarning("A null GameObject was found in the 'Wall Blocks' list on Plate 2!", this);
            }
        }
    }
}