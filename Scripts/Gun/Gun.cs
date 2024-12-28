using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;     // Bullet prefab
    public Transform shootPoint;        // Bullet spawn position
    public Camera mainCamera;           // Reference to the main camera
    public PlayerStatsModifier statsModifier; // Reference to PlayerStatsModifier

    public float baseBulletSpeed = 10f; // Base bullet speed
    public float baseBulletLifetime = 2f; // Base bullet lifetime
    public float baseFireRate = 0.5f;  // Base fire rate
    protected float nextFireTime;      // For controlling fire rate

    public virtual float AdjustedFireRate
    {
        get
        {
            // Check if statsModifier is null
            if (statsModifier == null)
            {
                Debug.LogError("StatsModifier is not assigned!");
                return baseFireRate; // Fallback to base fire rate
            }

            return baseFireRate / statsModifier.attackSpeedMultiplier;
        }
    }


    public virtual void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + AdjustedFireRate;

        // Shooting logic
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.SetActive(true);

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.velocity = shootDirection * baseBulletSpeed;
        else
            Debug.LogError("Rigidbody2D not found on bulletPrefab!");

        Destroy(bullet, baseBulletLifetime);
    }
}
