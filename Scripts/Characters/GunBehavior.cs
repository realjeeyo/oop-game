using UnityEngine;

public class GunBehavior : MonoBehaviour
{
    public GameObject bulletPrefab;   // Bullet prefab
    public Transform shootPoint;      // The point where bullets spawn
    public Camera mainCamera;         // Reference to the main camera
    public float bulletSpeed = 10f;   // Speed of the bullet

    void Update()
    {
        // Check for shooting input
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate the bullet at the shootPoint
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.SetActive(true);

        // Get the mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure it's in the same plane as the player

        // Calculate the shooting direction from shootPoint to the mouse position
        Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;

        Debug.Log($"Mouse Position: {mousePosition}");
        Debug.Log($"Shoot Point: {shootPoint.position}");
        Debug.Log($"Shoot Direction: {shootDirection}");

        // Set the bullet's velocity using Rigidbody2D
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = shootDirection * bulletSpeed;
            Debug.Log($"Bullet Velocity: {bulletRb.velocity}");
        }
        else
        {
            Debug.LogError("Rigidbody2D not found on bulletPrefab!");
        }

        // Destroy bullet after a set time
        Destroy(bullet, 2f);
    }

}
