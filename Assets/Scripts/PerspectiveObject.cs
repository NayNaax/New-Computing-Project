using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveObject : MonoBehaviour
{
    public Transform targetPoint; // The point the object should appear to be at
    public float minPlayerDistance = 2.0f; // Minimum distance to start repositioning
    public LayerMask collisionLayers; // Layers to check for collision
    public float dragDistance = 10f; // Maximum distance player can drag from
    public float dragSmoothing = 5f; // Smoothing factor for dragging (higher = more responsive)
    public float dragSpeed = 1f; // Speed multiplier for dragging
    public float collisionBuffer = 0.05f; // Buffer distance to keep from walls
    
    private Transform playerCamera;
    private bool playerInRange = false;
    private Vector3 initialPosition; // Store the initial position of the object
    private float objectDepth = 1f; // The depth of the object (adjust as needed)
    private bool isDragging = false;
    private Collider objectCollider;
    
    // For the new dragging system
    private Vector3 dragOffset;
    private float dragDepth;
    
    void Start()
    {
        playerCamera = Camera.main.transform;
        initialPosition = transform.position; // Store the initial position
        objectCollider = GetComponent<Collider>();
        
        // Set default collision layers if not assigned
        if (collisionLayers.value == 0)
            collisionLayers = Physics.DefaultRaycastLayers & ~(1 << gameObject.layer); // Exclude object's own layer
    }

    void Update()
    {
        // Check for mouse input
        if (Input.GetMouseButtonDown(0))
        {
            // Cast ray from camera through mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            // Check if ray hits this object
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                // Check if player is close enough to interact
                float distanceToPlayer = Vector3.Distance(playerCamera.position, transform.position);
                if (distanceToPlayer <= dragDistance)
                {
                    isDragging = true;
                    
                    // Calculate drag depth (distance from camera to object along view direction)
                    dragDepth = Vector3.Dot(transform.position - Camera.main.transform.position, Camera.main.transform.forward);
                    
                    // Calculate the offset between mouse and object in world space
                    Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(
                        new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
                    dragOffset = transform.position - mouseWorldPoint;
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            // Get the mouse position in world space based on the drag depth
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
                
            // Calculate the target position with the same offset as when we started dragging
            Vector3 targetPosition = mouseWorldPoint + dragOffset;
            
            // Store current position
            Vector3 originalPosition = transform.position;
            
            // Calculate the movement vector
            Vector3 movement = targetPosition - originalPosition;
            
            // Check if the movement would cause a collision before applying it
            if (!WouldCollide(movement))
            {
                // Try to move the object with smoothing
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * dragSmoothing * 10);
                
                // Double-check for collision after movement and revert if needed
                if (IsCollidingWithEnvironment())
                {
                    transform.position = originalPosition;
                }
            }
        }
        else if (playerInRange)
        {
            // Calculate the distance between player and object
            float distanceToPlayer = Vector3.Distance(playerCamera.position, transform.position);
            
            // Only reposition if the player is too close
            if (distanceToPlayer < minPlayerDistance)
            {
                // Ensure the object is positioned at a safe distance from player
                Vector3 desiredPosition = playerCamera.position + playerCamera.forward * minPlayerDistance;
                
                // Store current position
                Vector3 originalPosition = transform.position;
                
                // Calculate the movement vector
                Vector3 movement = desiredPosition - originalPosition;
                
                // Check if the movement would cause a collision
                if (!WouldCollide(movement))
                {
                    // Smooth transition to the new position
                    Vector3 newPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);
                    transform.position = newPosition;
                    
                    // Double-check for collision after movement and revert if needed
                    if (IsCollidingWithEnvironment())
                    {
                        transform.position = originalPosition;
                    }
                }
            }
        }
    }
    
    // Check if moving in the given direction would cause a collision
    private bool WouldCollide(Vector3 movement)
    {
        // If movement is very small, don't bother checking
        if (movement.magnitude < 0.001f)
            return false;
            
        // Cast a ray from the current position in the direction of movement
        Ray ray = new Ray(transform.position, movement.normalized);
        RaycastHit hit;
        
        // Check if we would hit something if we moved in this direction
        if (Physics.Raycast(ray, out hit, movement.magnitude + collisionBuffer, collisionLayers))
        {
            // Something is in the way!
            return true;
        }
        
        // Also check using box cast to account for the object's size
        if (Physics.BoxCast(
            objectCollider.bounds.center, 
            objectCollider.bounds.extents * 0.9f, 
            movement.normalized, 
            out hit, 
            transform.rotation, 
            movement.magnitude + collisionBuffer, 
            collisionLayers))
        {
            // We would hit something with our volume
            return true;
        }
        
        return false;
    }
    
    // Check if the object is currently colliding with environment objects
    private bool IsCollidingWithEnvironment()
    {
        // Get all overlapping colliders
        Collider[] overlaps = new Collider[10]; // Limit to 10 results for performance
        int numColliders = Physics.OverlapBoxNonAlloc(
            objectCollider.bounds.center,
            objectCollider.bounds.extents * 0.95f, // Slightly smaller to avoid self-collision
            overlaps,
            transform.rotation,
            collisionLayers
        );
        
        // Check each collider
        for (int i = 0; i < numColliders; i++)
        {
            // If it's not our own collider and not a trigger
            if (overlaps[i] != objectCollider && !overlaps[i].isTrigger)
            {
                return true; // Found an environment collision
            }
        }
        
        return false; // No collision
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