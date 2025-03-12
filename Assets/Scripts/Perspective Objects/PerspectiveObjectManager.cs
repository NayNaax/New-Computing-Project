using System.Collections.Generic;
using UnityEngine;

public class PerspectiveObjectManager : MonoBehaviour
{
    [System.Serializable]
    public class CubeData
    {
        public Transform cube;
        public Transform targetPoint;
        public PerspectiveObject perspectiveScript;
    }

    public List<CubeData> cubes = new List<CubeData>();

    public float dragDistance = 10f;
    public float dragSmoothing = 5f;
    public LayerMask collisionLayers;
    public float collisionBuffer = 0.05f;
    public float groundCheckDistance = 0.5f;
    public bool snapToGroundOnRelease = true;

    private CubeData draggingCube;
    private Vector3 dragOffset;
    private float dragDepth;

    void Start()
    {
        // Initialize all cubes and ensure they have the necessary components
        foreach (var cubeData in cubes)
        {
            if (cubeData.cube != null)
            {
                // Get or add PerspectiveObject component
                cubeData.perspectiveScript = cubeData.cube.GetComponent<PerspectiveObject>();
                if (cubeData.perspectiveScript == null)
                {
                    cubeData.perspectiveScript = cubeData.cube.gameObject.AddComponent<PerspectiveObject>();
                }
                
                // Configure the PerspectiveObject component
                cubeData.perspectiveScript.targetPoint = cubeData.targetPoint;
                cubeData.perspectiveScript.collisionLayers = collisionLayers;
                cubeData.perspectiveScript.dragDistance = dragDistance;
                cubeData.perspectiveScript.dragSmoothing = dragSmoothing;
                cubeData.perspectiveScript.collisionBuffer = collisionBuffer;
                cubeData.perspectiveScript.groundCheckDistance = groundCheckDistance;
                
                // Disable individual script updates - we'll control them from here
                cubeData.perspectiveScript.enabled = false;
                
                // Create target point if it doesn't exist
                if (cubeData.targetPoint == null)
                {
                    GameObject targetObj = new GameObject(cubeData.cube.name + "_Target");
                    cubeData.targetPoint = targetObj.transform;
                    cubeData.targetPoint.position = cubeData.cube.position;
                    cubeData.perspectiveScript.targetPoint = cubeData.targetPoint;
                }
            }
        }
    }

    void Update()
    {
        // Handle mouse input for dragging
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (var cubeData in cubes)
                {
                    if (hit.collider.gameObject == cubeData.cube.gameObject)
                    {
                        float distanceToPlayer = Vector3.Distance(Camera.main.transform.position, cubeData.cube.position);
                        if (distanceToPlayer <= dragDistance)
                        {
                            draggingCube = cubeData;
                            dragDepth = Vector3.Dot(cubeData.cube.position - Camera.main.transform.position, Camera.main.transform.forward);
                            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(
                                new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
                            dragOffset = cubeData.cube.position - mouseWorldPoint;
                            break;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (draggingCube != null && snapToGroundOnRelease)
            {
                // Try to snap to ground when releasing
                SnapToGround(draggingCube);
            }
            draggingCube = null;
        }

        // Handle dragging
        if (draggingCube != null)
        {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
            Vector3 targetPosition = mouseWorldPoint + dragOffset;
            
            // Get the rigidbody
            Rigidbody rb = draggingCube.cube.GetComponent<Rigidbody>();
            if (rb == null) return;
            
            // Check for collision
            if (!CheckCollision(targetPosition, draggingCube))
            {
                Vector3 smoothPosition = Vector3.Lerp(
                    draggingCube.cube.position, 
                    targetPosition, 
                    Time.deltaTime * dragSmoothing * 10f); // Increased smoothing factor
                
                rb.MovePosition(smoothPosition);
                
                // Also update target point if it exists
                if (draggingCube.targetPoint != null)
                {
                    draggingCube.targetPoint.position = smoothPosition;
                }
            }
        }
        
        // Handle player proximity for all cubes
        foreach (var cubeData in cubes)
        {
            if (cubeData != draggingCube) // Skip the cube being dragged
            {
                CheckPlayerProximity(cubeData);
            }
        }
    }
    
    private void CheckPlayerProximity(CubeData cubeData)
    {
        // Check if player is too close to the cube
        float distToPlayer = Vector3.Distance(Camera.main.transform.position, cubeData.cube.position);
        
        if (distToPlayer < cubeData.perspectiveScript.minPlayerDistance)
        {
            // Calculate position away from player
            Vector3 awayFromPlayer = cubeData.cube.position - Camera.main.transform.position;
            awayFromPlayer.y = 0; // Keep at same height
            
            if (awayFromPlayer.magnitude < 0.1f)
            {
                // If directly above/below player, move in player's forward direction
                awayFromPlayer = Camera.main.transform.forward;
                awayFromPlayer.y = 0;
            }
            
            awayFromPlayer.Normalize();
            Vector3 targetPosition = Camera.main.transform.position + 
                awayFromPlayer * cubeData.perspectiveScript.minPlayerDistance;
            
            // Check for collision
            if (!CheckCollision(targetPosition, cubeData))
            {
                // Get the rigidbody
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null) return;
                
                // Move the cube away from player smoothly
                rb.MovePosition(Vector3.Lerp(
                    cubeData.cube.position, 
                    targetPosition, 
                    Time.deltaTime * 5f));
                
                // Also update target point
                if (cubeData.targetPoint != null)
                {
                    cubeData.targetPoint.position = rb.position;
                }
            }
        }
    }
    
    private void SnapToGround(CubeData cubeData)
    {
        Collider collider = cubeData.cube.GetComponent<Collider>();
        if (collider == null) return;
        
        // Cast ray downward
        RaycastHit hit;
        if (Physics.Raycast(
            cubeData.cube.position, 
            Vector3.down, 
            out hit, 
            groundCheckDistance, 
            collisionLayers))
        {
            // Calculate position that would put the bottom of the collider on the surface
            float bottomOffset = collider.bounds.extents.y;
            Vector3 newPosition = new Vector3(
                cubeData.cube.position.x,
                hit.point.y + bottomOffset,
                cubeData.cube.position.z
            );
            
            // Make sure this position doesn't cause collisions
            if (!CheckCollision(newPosition, cubeData))
            {
                // Get the rigidbody
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null) return;
                
                // Apply the new position
                rb.MovePosition(newPosition);
                
                // Update target point
                if (cubeData.targetPoint != null)
                {
                    cubeData.targetPoint.position = newPosition;
                }
            }
        }
    }

    private bool CheckCollision(Vector3 targetPosition, CubeData cubeData)
    {
        Collider cubeCollider = cubeData.cube.GetComponent<Collider>();
        if (cubeCollider == null) return false;
        
        Collider[] hitColliders = Physics.OverlapBox(
            targetPosition, 
            cubeCollider.bounds.extents - new Vector3(collisionBuffer, collisionBuffer, collisionBuffer), 
            cubeData.cube.rotation, 
            collisionLayers
        );

        foreach (var hitCollider in hitColliders)
        {
            // Skip this cube's own collider and triggers
            if (hitCollider != cubeCollider && !hitCollider.isTrigger)
            {
                return true; // Collision detected
            }
        }
        return false; // No collision
    }
    
    void OnDrawGizmos()
    {
        // Draw debug visualization for currently dragging cube
        if (draggingCube != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(draggingCube.cube.position, 0.2f);
        }
        
        // Draw ground check rays
        foreach (var cubeData in cubes)
        {
            if (cubeData.cube != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(
                    cubeData.cube.position, 
                    cubeData.cube.position + Vector3.down * groundCheckDistance);
                
                // Draw target point
                if (cubeData.targetPoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(cubeData.targetPoint.position, 0.1f);
                }
            }
        }
    }
}
