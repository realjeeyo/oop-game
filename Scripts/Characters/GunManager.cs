using UnityEngine;

public class GunManager : MonoBehaviour
{
    public GameObject[] guns; // List of gun GameObjects to switch between
    private int currentGunIndex = 0;

    void Start()
    {
        // Activate the first gun and deactivate others
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(i == currentGunIndex);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to switch to the next gun
        {
            SwitchGun();
        }
    }

    void SwitchGun()
    {
        // Deactivate current gun
        guns[currentGunIndex].SetActive(false);

        // Move to the next gun in the list
        currentGunIndex = (currentGunIndex + 1) % guns.Length;

        // Activate the new gun
        guns[currentGunIndex].SetActive(true);
    }
}
