using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int price;                       // Price of the weapon
    public GameObject weaponPrefab;         // The actual weapon prefab
    private TextMeshProUGUI priceText;      // UI element for the price display

    private bool playerInRange = false;     // Flag to track if player is within range
    private PlayerWeaponController playerWeaponController; // Cached reference for efficiency

    public void InitializeItem(int itemPrice, GameObject prefab, TextMeshProUGUI priceLabel)
    {
        price = itemPrice;
        weaponPrefab = prefab;
        priceText = priceLabel;

        // Update the displayed price
        if (priceText != null)
        {
            priceText.text = $"${price}";
        }
        else
        {
            Debug.LogWarning("ShopItem: PriceText UI is missing for " + gameObject.name);
        }
    }

    private void Update()
    {
        // Handle input in Update to improve responsiveness
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            AttemptToPurchase(playerWeaponController);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerWeaponController = other.GetComponent<PlayerWeaponController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerWeaponController = null;
        }
    }

    private void AttemptToPurchase(PlayerWeaponController playerWeaponController)
    {
        if (playerWeaponController == null)
        {
            Debug.LogError("PlayerWeaponController is missing!");
            return;
        }

        if (PlayerInventory.Instance.coinCount < price)
        {
            Debug.Log("Not enough coins to purchase.");
            return;
        }

        // Deduct the price
        PlayerInventory.Instance.coinCount -= price;
        PlayerInventory.Instance.UpdateCoinUI();

        // Drop the old weapon and mark it as free
        GameObject droppedWeapon = playerWeaponController.DropCurrentWeapon();
        if (droppedWeapon != null)
        {
            ShopItem droppedItem = droppedWeapon.AddComponent<ShopItem>();
            droppedItem.price = 0; // Set to free
            droppedItem.weaponPrefab = droppedWeapon;
            droppedWeapon.tag = "FreeWeapon";
        }

        // Equip the new weapon
        playerWeaponController.PickupWeapon(weaponPrefab);

        // Destroy this shop item
        Destroy(gameObject);
    }
}
