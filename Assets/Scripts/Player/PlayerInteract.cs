using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Necessary for new Input System

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;

    [Tooltip("Maximum distance for interaction.")]
    public float distance = 3f;

    [Tooltip("LayerMask to filter interactable objects.")]
    public LayerMask mask;

    private PlayerUI playerUI;
    private InputManager inputManager;

    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        playerUI.UpdateText(string.Empty);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);

                // Check for interaction using the Input Action
                if (inputManager.onFoot.Interact.WasPressedThisFrame())
                {
                    interactable.Interact();
                }
            }
        }
    }
}