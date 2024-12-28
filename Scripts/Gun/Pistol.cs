using UnityEngine;

public class Pistol : Gun
{
    public int baseDamage = 1;

    private void Start()
    {
        baseFireRate = 0.5f;
        baseBulletSpeed = 10f;
    }

    public override void Shoot()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + AdjustedFireRate;

        if (shootPoint == null)
        {
            Debug.LogError("ShootPoint is not assigned!");
            return;
        }

        // Instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.SetActive(true);

        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            playerBullet.damage = Mathf.RoundToInt(baseDamage * statsModifier.damageMultiplier); // Apply damage multiplier
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;
            rb.velocity = shootDirection * baseBulletSpeed;
        }
        else
        {
            Debug.LogError("Rigidbody2D not found on bullet prefab!");
        }

        Destroy(bullet, baseBulletLifetime);
    }
}
