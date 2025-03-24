using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    private stateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }
    [SerializeField] private Transform player;
    // debugging purposes
    private string currentState;
    public path path;
    public float health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<stateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    public void ApplyDamage(RaycastHit hit, float damage)
    {
        enemy enemyComponent = hit.collider.GetComponent<enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damage);
            Debug.Log("Damage applied to enemy.");
        }
        else
        {
            Debug.LogWarning("Enemy component not found on hit object!");
        }
    }
}
