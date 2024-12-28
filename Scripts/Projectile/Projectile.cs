using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Properties")]
    public int damage = 1;                 // Damage the projectile deals
    public float lifeTime = 5f;            // Lifetime of the projectile
    public float knockbackForce = 0f;      // Optional knockback force on collision
    public bool destroyOnImpact = true;    // Whether to destroy the projectile on impact

    [Header("Collision Settings")]
    public string[] targetTags = { "Enemy", "Player" }; // Tags this projectile can interact with

    protected virtual void Start()
    {
        // Destroy the projectile after its lifetime
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has one of the specified tags
        foreach (string targetTag in targetTags)
        {
            if (other.CompareTag(targetTag))
            {
                HandleCollision(other, targetTag);
                break;
            }
        }
    }

    protected virtual void HandleCollision(Collider2D other, string targetTag)
    {
        if (targetTag == "Enemy")
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        else if (targetTag == "Player")
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Apply knockback if applicable
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null && knockbackForce > 0)
        {
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }

        // Destroy the projectile on impact, if enabled
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}
