using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modified GUN.cs
// Using GUN.cs [cite: uploaded:Assets/Scripts/GUN.cs]
public class GUN : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 1f;
    public Camera fpsCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    // --- Removed target-specific timer durations ---
    // Keep scoring if needed

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name + " (Tag: " + hit.collider.tag + ")");

            string hitTag = hit.collider.tag; // Get the tag of the object hit

            // --- MODIFIED: Check for IShootable interface ---
            IShootable shootableComponent = hit.collider.GetComponentInParent<IShootable>();
            if (shootableComponent != null)
            {
                 // Check if the hit tag corresponds to a target ring
                 if (hitTag == "TargetYellow" || hitTag == "TargetRed" || hitTag == "TargetBlue" || hitTag == "TargetWhite")
                 {
                    Debug.Log($"Found IShootable component. Activating reward for hit on tag: {hitTag}");
                    // Call the interface method, passing the specific tag that was hit
                    shootableComponent.ActivateReward(hitTag);
                 }
                 else
                 {
                    // Optional: Log if the hit object had IShootable but the specific collider tag wasn't a target ring
                    // Debug.Log($"Hit object with IShootable, but tag '{hitTag}' is not a recognized target ring.");
                 }
            }
            // --- END MODIFICATION ---
            else
            {
                 // Optional: Check for enemy or other interactables if needed
                 enemy enemyComponent = hit.collider.GetComponent<enemy>();
                 if (enemyComponent != null)
                 {
                     enemyComponent.TakeDamage(damage);
                     Debug.Log("Enemy health: " + enemyComponent.health);
                 }
            }


            // Spawn impact effect (no changes needed here)
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }
}