using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;

    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthbar;
    public Image backHealthbar;

    [Header("Damage Overlay")]
    public Image overlay; // Damage overlay image
    public float duration; // How long the overlay will be visible
    public float fadeSpeed; // How fast the overlay will fade

    private float durationTimer; // Timer to keep track of how long the overlay has been visible

    [Header("Damage Settings")]
    public float damageOnCollision = 10f; // Damage taken on collision with the enemy

    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (overlay.color.a > 0)
        {
            if (health < 30)
                return;

            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                // Fade out the overlay
                float tempAlpha = overlay.color.a;
                tempAlpha -= fadeSpeed * Time.deltaTime;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {
        float fillf = frontHealthbar.fillAmount;
        float fillb = backHealthbar.fillAmount;
        float hFraction = health / maxHealth;

        if (fillb > hFraction)
        {
            frontHealthbar.fillAmount = hFraction;
            backHealthbar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthbar.fillAmount = Mathf.Lerp(fillb, hFraction, percentComplete);
        }

        if (fillf < hFraction)
        {
            backHealthbar.fillAmount = hFraction;
            backHealthbar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthbar.fillAmount = Mathf.Lerp(fillf, hFraction, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0f;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    // Detect collision with the enemy
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with enemy. Taking damage.");
            TakeDamage(damageOnCollision);
        }
    }

    // Alternatively, use OnTriggerEnter if the enemy collider is set as a trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Triggered by enemy. Taking damage.");
            TakeDamage(damageOnCollision);
        }
    }
}
