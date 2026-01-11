using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [Min(0)] public float health = 100;

    [Header("UI")]
    public Canvas enemyUI;
    public Slider healthBar;

    private float currentHealth = 0;

    private void Start()
    {
        enemyUI.enabled = false;
        currentHealth = health;
        healthBar.maxValue = health;
        healthBar.value = currentHealth;
    }

    private void Update()
    {
        Transform playerPosition = GameObject.FindWithTag("Player").transform;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        agent.destination = playerPosition.position;

        enemyUI.enabled = currentHealth < health;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
