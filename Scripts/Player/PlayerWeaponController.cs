using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform gunHolder;        // Parent transform to hold the gun
    public GameObject currentWeapon;   // Currently equipped weapon
    public Transform gunTip;           // Reference to GunTip (shoot point)
    public PlayerStatsModifier playerStatsModifier; // Reference to player's stats modifier

    private void Start()
    {
        // Automatically find GunHolder and GunTip if not assigned
        if (gunHolder == null)
        {
            gunHolder = transform.Find("GunHolder");
            if (gunHolder == null)
                Debug.LogError("GunHolder not found! Ensure it exists as a child of the player.");
        }

        if (gunTip == null)
        {
            gunTip = gunHolder.Find("GunTip");
            if (gunTip == null)
                Debug.LogError("GunTip not found! Ensure it exists under GunHolder.");
        }
    }

    private void Update()
    {
        // Shooting input
        if (currentWeapon != null && Input.GetMouseButton(0)) // Left mouse button
        {
            Gun equippedGun = currentWeapon.GetComponent<Gun>();
            if (equippedGun != null && equippedGun.enabled)
            {
                if (equippedGun is AssaultRifle assaultRifle)
                {
                    assaultRifle.Fire();
                }
                else
                {
                    equippedGun.Shoot(); // For other types of guns
                }
            }
        }
    }

    // Equip a new weapon
    public void PickupWeapon(GameObject newWeaponPrefab)
    {
        if (newWeaponPrefab == null)
        {
            Debug.LogError("New weapon prefab is null!");
            return;
        }

        // Drop the current weapon
        DropCurrentWeapon();

        // Instantiate and equip the new weapon
        GameObject newWeapon = Instantiate(newWeaponPrefab, gunHolder);
        newWeapon.transform.localPosition = new Vector3(0.2f, 0.2f, 0); // Adjusted position
        float rotationAngle = transform.localScale.x > 0 ? 0f : 180f;    // Adjust rotation
        newWeapon.transform.localRotation = Quaternion.Euler(0, rotationAngle, 0);

        newWeapon.SetActive(true);

        // Assign to currentWeapon
        currentWeapon = newWeapon;

        // Assign GunTip as the shoot point and set stats modifier
        Gun gunScript = newWeapon.GetComponent<Gun>();
        if (gunScript != null)
        {
            gunScript.shootPoint = gunTip;
            gunScript.statsModifier = playerStatsModifier; // Assign player's stats modifier
            gunScript.enabled = true; // Ensure the gun script is active
            Debug.Log("ShootPoint and StatsModifier assigned dynamically.");
        }
        else
        {
            Debug.LogError("Gun script not found on the new weapon prefab!");
        }
    }

    // Drop the current weapon
    public GameObject DropCurrentWeapon()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("No weapon equipped to drop!");
            return null;
        }

        // Unparent the current weapon
        currentWeapon.transform.SetParent(null);
        currentWeapon.transform.position = transform.position; // Drop it at player's position

        // Re-enable collider for interaction
        Collider2D collider = currentWeapon.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Disable the weapon's script
        Gun gunScript = currentWeapon.GetComponent<Gun>();
        if (gunScript != null)
        {
            gunScript.enabled = false;
        }

        Debug.Log($"Dropped weapon: {currentWeapon.name}");

        // Clear the reference
        GameObject droppedWeapon = currentWeapon;
        currentWeapon = null;
        return droppedWeapon;
    }
}
