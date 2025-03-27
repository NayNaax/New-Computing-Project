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
    public float groundCheckDistance = 0.5f; // Distance to check for ground below the object
    
    private Transform playerCamera;
    private bool playerInRange = false;
    private Vector3 initialPosition; // Store the initial position of the object
    private bool isDragging = false;
    private Collider objectCollider;
    private Rigidbody rb;
    private Vector3 dragOffset;
    private float dragDepth;
    private Vector3 lastValidPosition; // Store the last position known to be valid
    
    void Start()
    {
        playerCamera = Camera.main.transform;
        initialPosition = transform.position;
        objectCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        lastValidPosition = transform.position;

        if (targetPoint == null)
        {
            Debug.LogError($"TargetPoint is not assigned for {gameObject.name}. Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        // Check for mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                float distanceToPlayer = Vector3.Distance(playerCamera.position, transform.position);
                if (distanceToPlayer <= dragDistance)
                {
                    isDragging = true;
                    dragDepth = Vector3.Dot(transform.position - Camera.main.transform.position, Camera.main.transform.forward);
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

        // Calculate player distance for various checks
        float distToPlayer = Vector3.Distance(playerCamera.position, transform.position);

        if (isDragging)
        {
            // Calculate target position
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
            Vector3 targetPosition = mouseWorldPoint + dragOffset;
            
            if (!CheckCollision(targetPosition))
            {
                rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * dragSmoothing));
                lastValidPosition = rb.position;
            }
        }
        else if (playerInRange && distToPlayer < minPlayerDistance)
        {
            // Reposition object if player is too close
            Vector3 previousPosition = transform.position;
            
            Vector3 awayFromPlayer = transform.position - playerCamera.position;
            awayFromPlayer.y = 0; 
            
            if (awayFromPlayer.magnitude < 0.1f)
            {
                awayFromPlayer = playerCamera.forward;
                awayFromPlayer.y = 0;
            }
            
            awayFromPlayer.Normalize();
            Vector3 targetPosition = playerCamera.position + awayFromPlayer * minPlayerDistance;
            
            if (!CheckCollision(targetPosition))
            {
                rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3f));
                lastValidPosition = rb.position;
            }
        }
    }
    
    private bool CheckCollision(Vector3 targetPosition)
    {
        Collider[] hitColliders = Physics.OverlapBox(
            targetPosition, 
            objectCollider.bounds.extents - new Vector3(collisionBuffer, collisionBuffer, collisionBuffer), 
            transform.rotation,
            collisionLayers
        );
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != objectCollider && !hitCollider.isTrigger)
            {
                return true; 
            }
        }
        
        return false; 
    }

    void OnDrawGizmos()
    {
        if (objectCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(objectCollider.bounds.center, 
                objectCollider.bounds.size - new Vector3(collisionBuffer*2, collisionBuffer*2, collisionBuffer*2));
        }
    }

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
