using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Message that is displayed to player when they are in range of the interactable object
    public string promptMessage;
    
    // Function that is called when the player interacts with the object
    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact()
    {
        // Template Function that is overwritten by subclasseses
    }
}