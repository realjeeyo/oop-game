using UnityEngine;

public class BasicGrunt : BaseEnemy
{
    public GameObject projectilePrefab;    // Projectile Prefab
    public float fireRate = 1f;            // Rate at which the enemy fires
    private float nextFireTime;            // Time tracking for the next fire

    protected override void Update()
    {
        base.Update();

        // Fire at regular intervals
        if (Time.time >= nextFireTime)
        {
            Attack();
            nextFireTime = Time.time + fireRate;
        }
    }
    public override void Attack()
    {
        if (player != null)
        {
            // Instantiate the projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.SetActive(true);

            // Get the direction from the enemy to the player
            Vector2 direction = (player.position - transform.position).normalized;

            Debug.Log($"Bullet Direction: {direction}");

            // Get the Rigidbody2D of the projectile to apply force
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Apply force to the projectile using AddForce
                rb.AddForce(direction * 5f, ForceMode2D.Impulse); // Adjust the 5f to control the speed
                Debug.Log($"Bullet Force Applied: {direction * 5f}");
            }
            else
            {
                Debug.LogError("Rigidbody2D not found on projectilePrefab!");
            }

            // Destroy the projectile after a certain time
            Destroy(projectile, 5f);
        }
    }


}
