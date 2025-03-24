using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUN : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 10f; // Damage dealt by the pistol
    public float range = 50f; // Maximum range of the pistol
    public float fireRate = 1f; // Shots per second
    public Camera fpsCamera; // Reference to the player's camera
    public ParticleSystem muzzleFlash; // Muzzle flash effect
    public GameObject impactEffect; // Impact effect prefab

    private float nextTimeToFire = 0f; // Time until the next shot can be fired

    void Update()
    {
        // Check if the left mouse button is pressed and the gun is ready to fire
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate; // Set the cooldown for the next shot
            Shoot();
        }
    }

    void Shoot()
    {
        // Play muzzle flash effect
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Perform a raycast to detect hits
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name); // Logs the name of the object hit

            // Check if the hit object has an enemy component
            enemy enemyComponent = hit.collider.GetComponent<enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(damage); // Apply damage to the enemy
                Debug.Log("Enemy health: " + enemyComponent.health);
            }

            // Spawn impact effect at the hit point
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f); // Destroy the impact effect after 2 seconds
            }
        }
    }
}
