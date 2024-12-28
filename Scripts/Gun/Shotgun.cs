using UnityEngine;

public class Shotgun : Gun
{
    public int basePellets = 3;
    public float spreadAngle = 15f;
    public int baseDamage = 5;

    private void Start()
    {
        baseFireRate = 1f;
        baseBulletSpeed = 8f;
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

        for (int i = 0; i < basePellets; i++)
        {
            GameObject pellet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            pellet.SetActive(true);

            PlayerBullet playerBullet = pellet.GetComponent<PlayerBullet>();
            if (playerBullet != null)
            {
                playerBullet.damage = Mathf.RoundToInt(baseDamage * statsModifier.damageMultiplier); // Apply damage multiplier
            }

            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;

                Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;
                float angle = Random.Range(-spreadAngle, spreadAngle);
                shootDirection = Quaternion.Euler(0, 0, angle) * shootDirection;

                rb.velocity = shootDirection * baseBulletSpeed;
            }

            Destroy(pellet, baseBulletLifetime);
        }
    }
}
