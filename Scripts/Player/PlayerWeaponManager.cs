using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Transform weaponHolder; // Where the weapon is held
    private Gun currentGun;

    public void EquipGun(Gun newGun)
    {
        if (currentGun != null)
        {
            Destroy(currentGun.gameObject); // Remove the old gun
        }

        currentGun = Instantiate(newGun, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        currentGun.mainCamera = Camera.main; // Assign the main camera
    }
}
