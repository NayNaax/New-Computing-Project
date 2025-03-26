using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using GUN.cs [cite: uploaded:Scripts/GUN.cs]
public class GUN : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 1f;
    public Camera fpsCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Target Interaction Settings")]
    // --- Define Timer Durations ---
    public float durationYellow = 12f;
    public float durationRed = 8f;
    public float durationBlue = 5f;
    public float durationWhite = 3f;
    // -----------------------------

    // --- Optional Scoring ---
    // public int score = 0;
    // public int scoreWhite = 10;
    // public int scoreBlue = 25;
    // public int scoreRed = 50;
    // public int scoreYellow = 100;
    // -----------------------

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

            bool targetRingHit = false;      // Flag if a valid ring was hit
            float durationToPass = 0f;      // Duration determined by the hit ring

            string hitTag = hit.collider.tag; // Get the tag of the object hit

            // Determine duration based on the tag
            if (hitTag == "TargetYellow")
            {
                // score += scoreYellow; // Optional scoring
                Debug.Log("Hit Yellow Rim!");
                durationToPass = durationYellow;
                targetRingHit = true;
            }
            else if (hitTag == "TargetRed")
            {
                // score += scoreRed;
                Debug.Log("Hit Red Rim!");
                durationToPass = durationRed;
                targetRingHit = true;
            }
            else if (hitTag == "TargetBlue")
            {
                 // score += scoreBlue;
                 Debug.Log("Hit Blue Rim!");
                 durationToPass = durationBlue;
                 targetRingHit = true;
            }
            else if (hitTag == "TargetWhite")
            {
                 // score += scoreWhite;
                 Debug.Log("Hit White Rim!");
                 durationToPass = durationWhite;
                 targetRingHit = true;
            }

            // If a valid ring was hit, activate the parent's timer
            if (targetRingHit)
            {
                // Find the ShootableTarget script on the PARENT of the hit ring
                ShootableTarget parentTargetScript = hit.collider.GetComponentInParent<ShootableTarget>();
                if (parentTargetScript != null)
                {
                    // Call the Activate method, passing the determined duration
                    parentTargetScript.Activate(durationToPass);
                }
                else
                {
                    Debug.LogWarning("Hit a target ring, but couldn't find ShootableTarget script on parent!", hit.collider.gameObject);
                }
            }

            // --- Keep Enemy Logic ---
            enemy enemyComponent = hit.collider.GetComponent<enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(damage);
                Debug.Log("Enemy health: " + enemyComponent.health);
            }
            // ------------------------

            // Spawn impact effect
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }
}