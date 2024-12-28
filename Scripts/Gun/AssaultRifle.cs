using UnityEngine;

public class AssaultRifle : Gun
{
    public int baseDamage = 1;                // Base damage for Assault Rifle
    public float ARFireRate = 3f;      // Fire rate for Assault Rifle
    public int price = 3;                    // Price for the shop

    private void Start()
    {
        nextFireTime = 0f;                   // Initialize next fire time
        mainCamera = Camera.main;           // Assign the main camera
    }

    // Fire method that checks fire rate and triggers Shoot
    public void Fire()
    {
        if (Time.time >= nextFireTime) // Check fire rate
        {
            nextFireTime = Time.time + AdjustedFireRate; // Calculate next fire time
            Shoot(); // Call the base class's Shoot method
        }
    }

    public override void Shoot()
    {
        if (shootPoint == null)
        {
            Debug.LogError("ShootPoint is not assigned!");
            return;
        }

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.SetActive(true);

        // Set bullet damage
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            playerBullet.damage = Mathf.RoundToInt(baseDamage * statsModifier.damageMultiplier); // Apply damage multiplier
        }

        // Add velocity to the bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure the bullet stays in the same plane

            Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;
            rb.velocity = shootDirection * baseBulletSpeed; // Apply base bullet speed
        }
        else
        {
            Debug.LogError("Rigidbody2D not found on bullet prefab!");
        }

        // Destroy the bullet after its lifetime
        Destroy(bullet, baseBulletLifetime);
    }
}
