using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthbar;
    public Image backHealthbar;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if(Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(Random.Range(5, 10));
        }
          if(Input.GetKeyDown(KeyCode.S))
        {
            Heal(Random.Range(5, 10));
        }
    }
    public void UpdateHealthUI()
    {
        Debug.Log(health);
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
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
}
