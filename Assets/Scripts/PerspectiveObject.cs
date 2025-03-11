using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveObject : MonoBehaviour
{
    public Transform targetPoint; // The point the object should appear to be at
    private Transform playerCamera;
    private bool playerInRange = false;
    private Vector3 initialPosition; // Store the initial position of the object
    private float objectDepth = 1f; // The depth of the object (adjust as needed)

    void Start()
    {
        playerCamera = Camera.main.transform;
        initialPosition = transform.position; // Store the initial position
    }

    void Update()
    {
        if (playerInRange)
        {
            // Calculate the direction from the camera to the target point
            Vector3 direction = targetPoint.position - playerCamera.position;

            // Calculate the desired position without considering the object's depth
            Vector3 desiredPosition = playerCamera.position + direction.normalized * Vector3.Distance(playerCamera.position, initialPosition);

            // Check if the desired position is behind the player
            if (Vector3.Dot(direction, transform.forward) < 0)
            {
                // Adjust the desired position to be in front of the player
                desiredPosition += transform.forward * objectDepth;
            }

            // Set the object's position
            transform.position = desiredPosition;
        }
    }

    // OnTriggerEnter and OnTriggerExit are called when the player enters and exits the trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}