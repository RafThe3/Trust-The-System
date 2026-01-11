using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        // Detect if we hit an enemy
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>().TakeDamage(damage);
        }

        // Destroy bullet on impact
        Destroy(gameObject);
    }
}
