using UnityEngine;
using System.Collections;

public class BurstRifle : Gun
{
    [Header("Burst Rifle Settings")]
    public int baseBurstCount = 3;       // Number of shots per burst
    public float baseBurstDelay = 0.1f;  // Delay between shots in a burst
    public float burstCooldown = 1f;     // Delay after completing a burst
    public int baseDamage = 3;           // Damage per bullet

    private bool isBursting = false;     // Prevent overlapping bursts
    private bool isOnCooldown = false;   // Prevent firing during burst cooldown

    private void Start()
    {
        baseFireRate = burstCooldown; // Cooldown after burst
    }

    public override void Shoot()
    {
        // Prevent shooting if bursting, on cooldown, or before the next allowed fire time
        if (isBursting || isOnCooldown || Time.time < nextFireTime)
            return;

        // Start burst sequence
        nextFireTime = Time.time + burstCooldown; // Set cooldown after burst
        StartCoroutine(BurstShoot());
    }

    private IEnumerator BurstShoot()
    {
        isBursting = true; // Prevent multiple bursts overlapping
        isOnCooldown = true; // Prevent firing during cooldown

        float adjustedBurstDelay = baseBurstDelay / statsModifier.attackSpeedMultiplier; // Adjust delay with attack speed

        for (int i = 0; i < baseBurstCount; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(adjustedBurstDelay);
        }

        isBursting = false; // Burst complete

        // Enforce a full cooldown after the burst
        yield return new WaitForSeconds(burstCooldown - (adjustedBurstDelay * baseBurstCount));
        isOnCooldown = false; // Ready for the next burst
    }

    private void FireBullet()
    {
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

    private void Update()
    {
        // Fire only on Mouse Click Down, not continuously while holding
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
}
