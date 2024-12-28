using UnityEngine;

public class SniperRifle : Gun
{
    public int baseDamage = 5;
    public float zoomFactor = 2f;

    private void Start()
    {
        baseFireRate = 0.75f;
        baseBulletSpeed = 20f;
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

        Destroy(bullet, baseBulletLifetime);
    }

    public void ToggleZoom(bool isZooming)
    {
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = isZooming
                ? mainCamera.orthographicSize / zoomFactor
                : mainCamera.orthographicSize * zoomFactor;
        }
    }
}
