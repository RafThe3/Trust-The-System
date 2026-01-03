using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [Min(0)] public float health = 100;

    private void Update()
    {
        Transform playerPosition = GameObject.FindWithTag("Player").transform;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        agent.destination = playerPosition.position;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
