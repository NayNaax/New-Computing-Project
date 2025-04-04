using UnityEngine;
using UnityEngine.Events; // For the optional death event

// Place this script on your Level 3 Boss GameObject
public class Level3Boss : MonoBehaviour
{
    [Header("Boss Stats")]
    [Tooltip("Total health points for the boss.")]
    public float health = 500f; // Example health value

    [Header("References")]
    [Tooltip("Drag the specific 'End Button Quit' UI GameObject here.")]
    public EndButtonQuit endButtonQuitReference; // Assign in Inspector

    [Header("Events")]
    [Tooltip("Optional: Event to trigger when the boss dies.")]
    public UnityEvent onBossDeath;

    private bool isDead = false;

    // Call this method from your GUN script or other damage sources
    public void TakeDamage(float damage)
    {
        if (isDead) return; // Don't take damage if already dead

        health -= damage;
        Debug.Log($"Level 3 Boss took {damage} damage. Health remaining: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent Die() from running multiple times
        isDead = true;

        Debug.Log("Level 3 Boss defeated!");

        // Trigger the End Button Quit to appear
        if (endButtonQuitReference != null)
        {
            Debug.Log("Attempting to show End Button Quit...");
            endButtonQuitReference.ShowButton();
        }
        else
        {
            Debug.LogError("EndButtonQuit reference is NOT set on the Level3Boss script in the Inspector!");
        }

        // Trigger any other events linked to the boss's death
        onBossDeath?.Invoke();

        // Optional: Add explosion effect, disable components, play animation, etc.

        // Destroy the boss GameObject after a short delay (optional)
        // Destroy(gameObject, 2f); // Example delay
        Destroy(gameObject); // Or destroy immediately
    }
}