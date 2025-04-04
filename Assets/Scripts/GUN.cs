using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUN : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 1f;
    public Camera fpsCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

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

            string hitTag = hit.collider.tag;

            // 1. Check for Level 3 Boss FIRST
            Level3Boss boss = hit.collider.GetComponentInParent<Level3Boss>();
            if (boss != null)
            {
                Debug.Log("Hit Level 3 Boss!");
                boss.TakeDamage(damage);
            }
            else
            {
                // 2. Check for IShootable (Targets)
                IShootable shootableComponent = hit.collider.GetComponentInParent<IShootable>();
                if (shootableComponent != null)
                {
                    // Check if the hit tag corresponds to a target ring
                    if (hitTag == "TargetYellow" || hitTag == "TargetRed" || hitTag == "TargetBlue" || hitTag == "TargetWhite")
                    {
                        Debug.Log($"Found IShootable component. Activating reward for hit on tag: {hitTag}");
                        shootableComponent.ActivateReward(hitTag);
                    }
                    
                }
                else
                {
                    // 3. Check for generic enemy component
                    enemy enemyComponent = hit.collider.GetComponentInParent<enemy>();
                    if (enemyComponent != null)
                    {
                         Debug.Log("Hit generic enemy.");
                         enemyComponent.TakeDamage(damage);
                         Debug.Log("Enemy health: " + enemyComponent.health);
                    }
                    else
                    {
                        Debug.Log("Hit object is not Boss, IShootable, or generic Enemy.");
                    }
                }
            }

            // Spawn impact effect
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }
}