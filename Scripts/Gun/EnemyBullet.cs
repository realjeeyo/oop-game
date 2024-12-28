using UnityEngine;

public class EnemyBullet : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions with objects tagged as "Enemy"
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Ignored collision with Enemy");
            return;
        }

        // Call the base method to handle other collision types
        base.OnTriggerEnter2D(other);
    }
}
