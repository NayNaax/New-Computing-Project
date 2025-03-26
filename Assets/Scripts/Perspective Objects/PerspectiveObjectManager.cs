using System.Collections.Generic;
using UnityEngine;

// Using PerspectiveObjectManager.cs [cite: uploaded:Scripts/Perspective Objects/PerspectiveObjectManager.cs]
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
    public float collisionBuffer = 0.1f;
    public float groundCheckDistance = 0.5f;
    public bool snapToGroundOnRelease = true;

    private CubeData draggingCube;
    private Vector3 dragOffset;
    private float dragDepth;

    private InputManager inputManager;
    // Removed ResizableObject reference as it wasn't used in the drag logic here
    // private ResizableObject resizableObject;

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>(); // Find the InputManager in the scene

        foreach (var cubeData in cubes)
        {
            if (cubeData.cube != null)
            {
                // --- (Rigidbody, ResizableObject, PerspectiveObject setup remains the same) ---
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = cubeData.cube.gameObject.AddComponent<Rigidbody>();
                }
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.drag = 0.1f;
                rb.angularDrag = 0.1f;

                if (cubeData.cube.GetComponent<ResizableObject>() == null)
                {
                    cubeData.cube.gameObject.AddComponent<ResizableObject>();
                }

                cubeData.perspectiveScript = cubeData.cube.GetComponent<PerspectiveObject>()
                                             ?? cubeData.cube.gameObject.AddComponent<PerspectiveObject>();

                cubeData.perspectiveScript.targetPoint = cubeData.targetPoint;
                cubeData.perspectiveScript.collisionLayers = collisionLayers;
                cubeData.perspectiveScript.dragDistance = dragDistance;
                cubeData.perspectiveScript.dragSmoothing = dragSmoothing;
                cubeData.perspectiveScript.collisionBuffer = collisionBuffer;
                cubeData.perspectiveScript.groundCheckDistance = groundCheckDistance;
                cubeData.perspectiveScript.enabled = false; // Assuming this should remain false initially

                if (cubeData.targetPoint == null && cubeData.cube != null) // Added null check for cube
                {
                    GameObject targetObj = new GameObject(cubeData.cube.name + "_Target");
                    cubeData.targetPoint = targetObj.transform;
                    cubeData.targetPoint.position = cubeData.cube.position;
                    if (cubeData.perspectiveScript != null) // Check perspectiveScript exists
                    {
                        cubeData.perspectiveScript.targetPoint = cubeData.targetPoint;
                    }
                }
                 // Removed assigning to local resizableObject here
            }
        }
    }

    void Update()
    {
        // --- START INPUT MODIFICATION ---
        // Use Right Mouse Button (1) to start dragging
        if (Input.GetMouseButtonDown(1)) // Changed from 0 to 1
        // --- END INPUT MODIFICATION ---
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (var cubeData in cubes)
                {
                    // Ensure cubeData and cubeData.cube are not null before accessing
                    if (cubeData != null && cubeData.cube != null && hit.collider.gameObject == cubeData.cube.gameObject)
                    {
                        float distanceToPlayer = Vector3.Distance(Camera.main.transform.position, cubeData.cube.position);
                        if (distanceToPlayer <= dragDistance)
                        {
                            draggingCube = cubeData;
                            // Calculate drag offset based on current camera perspective
                            dragDepth = Vector3.Dot(cubeData.cube.position - Camera.main.transform.position, Camera.main.transform.forward);
                            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
                            dragOffset = cubeData.cube.position - mouseWorldPoint;
                            break; // Found the cube to drag, exit loop
                        }
                    }
                }
            }
        }

        // --- START INPUT MODIFICATION ---
        // Use Right Mouse Button (1) to stop dragging
        if (Input.GetMouseButtonUp(1)) // Changed from 0 to 1
        // --- END INPUT MODIFICATION ---
        {
            if (draggingCube != null && snapToGroundOnRelease)
            {
                SnapToGround(draggingCube);
            }
            draggingCube = null; // Stop dragging
        }

        // --- Dragging Logic (remains mostly the same) ---
        if (draggingCube != null)
        {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
            Vector3 targetPosition = mouseWorldPoint + dragOffset;

            Rigidbody rb = draggingCube.cube.GetComponent<Rigidbody>();
            if (rb == null) return; // Should not happen based on Start(), but good check

            // Use MovePosition for smoother physics interaction if Rigidbody is involved
             if (!CheckCollision(targetPosition, draggingCube)) // Check for collisions before moving
             {
                 Vector3 smoothPosition = Vector3.Lerp(draggingCube.cube.position, targetPosition, Time.deltaTime * dragSmoothing * 10f); // Adjusted smoothing
                 rb.MovePosition(smoothPosition);

                 // Update target point if it exists
                 if (draggingCube.targetPoint != null)
                 {
                     draggingCube.targetPoint.position = smoothPosition;
                 }
             }


            // Handle Resizing (using Input Manager if available)
            ResizableObject currentResizable = draggingCube.cube.GetComponent<ResizableObject>();
            if (currentResizable != null && inputManager != null)
            {
                if (inputManager.onFoot.ResizeUp.IsPressed())
                {
                    Debug.Log($"Resizing up {draggingCube.cube.name}");
                    currentResizable.ResizeUp();
                }
                if (inputManager.onFoot.ResizeDown.IsPressed())
                {
                    Debug.Log($"Resizing down {draggingCube.cube.name}");
                    currentResizable.ResizeDown();
                }
            }
        }

        // --- Proximity Check (remains the same) ---
         foreach (var cubeData in cubes)
         {
             // Ensure cubeData and cubeData.cube are not null
             if (cubeData != null && cubeData.cube != null && cubeData != draggingCube)
             {
                 CheckPlayerProximity(cubeData);
             }
         }
    }

    // --- Helper methods (CheckPlayerProximity, SnapToGround, CheckCollision, OnDrawGizmos) remain the same ---
    // ... (Make sure these methods are present and unchanged from your original script) ...

    private void CheckPlayerProximity(CubeData cubeData)
    {
        // Ensure perspectiveScript exists before accessing minPlayerDistance
        if (cubeData.perspectiveScript == null) return;

        float distToPlayer = Vector3.Distance(Camera.main.transform.position, cubeData.cube.position);

        if (distToPlayer < cubeData.perspectiveScript.minPlayerDistance)
        {
            Vector3 awayFromPlayer = cubeData.cube.position - Camera.main.transform.position;
            awayFromPlayer.y = 0; // Keep movement horizontal

            if (awayFromPlayer.magnitude < 0.1f) // Avoid issues if player is exactly at the cube center
            {
                awayFromPlayer = Camera.main.transform.forward;
                awayFromPlayer.y = 0;
            }

            awayFromPlayer.Normalize();
            // Calculate target position based on min distance from the camera
            Vector3 targetPosition = Camera.main.transform.position + awayFromPlayer * cubeData.perspectiveScript.minPlayerDistance;

            // Check collision before moving
            if (!CheckCollision(targetPosition, cubeData))
            {
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null) return;

                // Use MovePosition for consistency
                rb.MovePosition(Vector3.Lerp(cubeData.cube.position, targetPosition, Time.deltaTime * 5f));

                // Update target point if it exists
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
        if (collider == null) return; // Need a collider to calculate bounds

        RaycastHit hit;
        // Raycast downwards from the cube's current position
        if (Physics.Raycast(cubeData.cube.position, Vector3.down, out hit, groundCheckDistance, collisionLayers))
        {
            float bottomOffset = collider.bounds.extents.y; // Distance from center to bottom
            // Target position is hit point + offset to place bottom of collider at hit point
            Vector3 newPosition = new Vector3(cubeData.cube.position.x, hit.point.y + bottomOffset, cubeData.cube.position.z);

            // Check collision before snapping
            if (!CheckCollision(newPosition, cubeData))
            {
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null) return;

                rb.MovePosition(newPosition); // Snap position

                // Update target point if it exists
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
        if (cubeCollider == null) return false; // Cannot check collision without a collider

        // Define the half-extents for the BoxCast, slightly smaller than the collider bounds for buffer
        Vector3 halfExtents = cubeCollider.bounds.extents - new Vector3(collisionBuffer / 2f, collisionBuffer / 2f, collisionBuffer / 2f);
        // Ensure halfExtents are not negative
        halfExtents = Vector3.Max(halfExtents, Vector3.zero);


        // Check for overlap at the target position first
        Collider[] overlaps = Physics.OverlapBox(targetPosition, halfExtents, cubeData.cube.rotation, collisionLayers);
        foreach (var hit in overlaps)
        {
             if (hit != cubeCollider && !hit.isTrigger)
             {
                 // Debug.Log($"Collision predicted with {hit.name} at target position {targetPosition}");
                 return true; // Collision detected at the destination
             }
        }


        // Optional: Cast slightly if overlap check isn't sufficient (might be redundant)
        /*
        Vector3 movementDelta = targetPosition - cubeData.cube.position;
        float movementDistance = movementDelta.magnitude;

        if (movementDistance > 0.001f) // Only cast if moving
        {
             RaycastHit[] hits = Physics.BoxCastAll(
                cubeData.cube.position, // Start from current position
                halfExtents,
                movementDelta.normalized, // Cast in direction of movement
                cubeData.cube.rotation,
                movementDistance, // Only cast distance needed
                collisionLayers
            );

            foreach (var hit in hits)
            {
                if (hit.collider != cubeCollider && !hit.collider.isTrigger)
                {
                    // Debug.Log($"Collision predicted with {hit.collider.name} during movement");
                    return true; // Collision detected during movement
                }
            }
        }
        */

        return false; // No collision detected
    }

     void OnDrawGizmos()
     {
         // Draw wireframes for debugging
         // ... (Gizmos code remains the same) ...
     }

}