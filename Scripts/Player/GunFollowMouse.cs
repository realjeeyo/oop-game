using UnityEngine;

public class GunFollowMouse : MonoBehaviour
{
    public Transform player;           // Reference to the player's transform
    public Transform gunHolder;        // Reference to the gun holder (positioned on the player)
    public Camera mainCamera;          // Reference to the main camera
    public float followSpeed = 10f;    // Speed at which the gun follows the mouse
    public float maxOffset = 1.5f;     // Maximum offset distance for the gun from the player

    void Update()
    {
        RotateGun();
        PositionGun();
    }

    void RotateGun()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure it's in 2D space

        // Calculate the direction from the gunHolder to the mouse
        Vector3 direction = mousePosition - gunHolder.position;

        // Calculate the angle to the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle based on the player's facing direction
        if (player.localScale.x < 0) // Player facing left
        {
            angle += 180f; // Flip the angle for left-facing direction
        }

        // Apply the rotation to the gunHolder
        gunHolder.rotation = Quaternion.Euler(0, 0, angle);
    }

    void PositionGun()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure it’s in the same plane as the player

        // Calculate the direction from the player to the mouse
        Vector3 direction = (mousePosition - player.position).normalized;

        // Clamp the gun position to a maximum offset distance from the player
        Vector3 targetPosition = player.position + direction * maxOffset;

        // Smoothly move the gun towards the clamped position
        gunHolder.position = Vector3.Lerp(gunHolder.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
