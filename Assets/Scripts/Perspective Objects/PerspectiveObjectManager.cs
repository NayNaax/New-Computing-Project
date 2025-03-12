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
    public float collisionBuffer = 0.1f;
    public float groundCheckDistance = 0.5f;
    public bool snapToGroundOnRelease = true;

    private CubeData draggingCube;
    private Vector3 dragOffset;
    private float dragDepth;

    // Add these
    private InputManager inputManager;
    private ResizableObject resizableObject;

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>(); // Find the InputManager in the scene

        foreach (var cubeData in cubes)
        {
            if (cubeData.cube != null)
            {
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = cubeData.cube.gameObject.AddComponent<Rigidbody>();
                }

                // Configure Rigidbody for better collision
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.drag = 0.1f;
                rb.angularDrag = 0.1f;

                // Make sure each cube has the ResizableObject component
                if (cubeData.cube.GetComponent<ResizableObject>() == null)
                {
                    cubeData.cube.gameObject.AddComponent<ResizableObject>();
                }

                // Set up PerspectiveObject component
                cubeData.perspectiveScript = cubeData.cube.GetComponent<PerspectiveObject>()
                                             ?? cubeData.cube.gameObject.AddComponent<PerspectiveObject>();

                cubeData.perspectiveScript.targetPoint = cubeData.targetPoint;
                cubeData.perspectiveScript.collisionLayers = collisionLayers;
                cubeData.perspectiveScript.dragDistance = dragDistance;
                cubeData.perspectiveScript.dragSmoothing = dragSmoothing;
                cubeData.perspectiveScript.collisionBuffer = collisionBuffer;
                cubeData.perspectiveScript.groundCheckDistance = groundCheckDistance;

                cubeData.perspectiveScript.enabled = false;

                if (cubeData.targetPoint == null)
                {
                    GameObject targetObj = new GameObject(cubeData.cube.name + "_Target");
                    cubeData.targetPoint = targetObj.transform;
                    cubeData.targetPoint.position = cubeData.cube.position;
                    cubeData.perspectiveScript.targetPoint = cubeData.targetPoint;
                }

                // Add ResizableObject to the cube
                resizableObject = cubeData.cube.GetComponent<ResizableObject>() ?? cubeData.cube.gameObject.AddComponent<ResizableObject>();
            }
        }
    }

    void Update()
    {
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
                            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
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
                SnapToGround(draggingCube);
            }
            draggingCube = null;
        }

        if (draggingCube != null)
        {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
            Vector3 targetPosition = mouseWorldPoint + dragOffset;

            Rigidbody rb = draggingCube.cube.GetComponent<Rigidbody>();
            if (rb == null) return;

            if (!CheckCollision(targetPosition, draggingCube))
            {
                Vector3 smoothPosition = Vector3.Lerp(draggingCube.cube.position, targetPosition, Time.deltaTime * dragSmoothing * 10f);
                rb.MovePosition(smoothPosition);

                if (draggingCube.targetPoint != null)
                {
                    draggingCube.targetPoint.position = smoothPosition;
                }
            }
            
            // IMPORTANT CHANGE: Get the ResizableObject component from the CURRENT dragging cube
            ResizableObject currentResizable = draggingCube.cube.GetComponent<ResizableObject>();
            
            // Check for resize input with the correct component
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

        foreach (var cubeData in cubes)
        {
            if (cubeData != draggingCube)
            {
                CheckPlayerProximity(cubeData);
            }
        }
    }

    private void CheckPlayerProximity(CubeData cubeData)
    {
        float distToPlayer = Vector3.Distance(Camera.main.transform.position, cubeData.cube.position);

        if (distToPlayer < cubeData.perspectiveScript.minPlayerDistance)
        {
            Vector3 awayFromPlayer = cubeData.cube.position - Camera.main.transform.position;
            awayFromPlayer.y = 0;

            if (awayFromPlayer.magnitude < 0.1f)
            {
                awayFromPlayer = Camera.main.transform.forward;
                awayFromPlayer.y = 0;
            }

            awayFromPlayer.Normalize();
            Vector3 targetPosition = Camera.main.transform.position + awayFromPlayer * cubeData.perspectiveScript.minPlayerDistance;

            if (!CheckCollision(targetPosition, cubeData))
            {
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null) return;

                rb.MovePosition(Vector3.Lerp(cubeData.cube.position, targetPosition, Time.deltaTime * 5f));

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

        RaycastHit hit;
        if (Physics.Raycast(cubeData.cube.position, Vector3.down, out hit, groundCheckDistance, collisionLayers))
        {
            float bottomOffset = collider.bounds.extents.y;
            Vector3 newPosition = new Vector3(cubeData.cube.position.x, hit.point.y + bottomOffset, cubeData.cube.position.z);

            if (!CheckCollision(newPosition, cubeData))
            {
                Rigidbody rb = cubeData.cube.GetComponent<Rigidbody>();
                if (rb == null) return;

                rb.MovePosition(newPosition);

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
        
        // Get the movement direction and distance
        Vector3 movementDelta = targetPosition - cubeData.cube.position;
        float movementDistance = movementDelta.magnitude;
        
        if (movementDistance < 0.0001f) return false; // No significant movement
        
        // Cast the collider in the movement direction to check for collisions
        RaycastHit[] hits = Physics.BoxCastAll(
            cubeData.cube.position,
            cubeCollider.bounds.extents - new Vector3(collisionBuffer/2, collisionBuffer/2, collisionBuffer/2),
            movementDelta.normalized,
            cubeData.cube.rotation,
            movementDistance,
            collisionLayers
        );

        foreach (var hit in hits)
        {
            // Skip this cube's own collider and triggers
            if (hit.collider != cubeCollider && !hit.collider.isTrigger)
            {
                return true; // Collision detected
            }
        }
        
        return false; // No collision
    }

    void OnDrawGizmos()
    {
        if (draggingCube != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(draggingCube.cube.position, 0.2f);
        }

        foreach (var cubeData in cubes)
        {
            if (cubeData.cube != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(cubeData.cube.position, cubeData.cube.position + Vector3.down * groundCheckDistance);

                if (cubeData.targetPoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(cubeData.targetPoint.position, 0.1f);
                }
            }
        }
    }
}