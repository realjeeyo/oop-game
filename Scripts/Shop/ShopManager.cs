using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Item Pool")]
    public List<GameObject> weaponPrefabs;
    public List<GameObject> powerupPrefabs;
    public List<GameObject> consumablePrefabs;

    [Header("Shop Slots")]
    public Transform[] shopSlots;
    public TextMeshProUGUI[] priceLabels;

    [Header("Price Settings")]
    public Vector2Int consumablePriceRange = new Vector2Int(2, 3);
    public Vector2Int powerupPriceRange = new Vector2Int(5, 8);
    public Vector2Int weaponPriceRange = new Vector2Int(6, 15);

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        PositionShopSlots();
        InitializeShopItems();
    }

    /// <summary>
    /// Dynamically position the shop slots and price labels based on camera bounds.
    /// </summary>
    private void PositionShopSlots()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found for positioning shop slots!");
            return;
        }

        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector3 camCenter = mainCamera.transform.position;

        float leftX = camCenter.x - (camWidth / 4);
        float rightX = camCenter.x + (camWidth / 4);

        if (shopSlots.Length < 3 || priceLabels.Length < 3)
        {
            Debug.LogError("Ensure there are at least 3 shop slots and price labels assigned in the inspector!");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            float xPos = Mathf.Lerp(leftX, rightX, (i + 1) / 4f);
            float yPos = camCenter.y;

            shopSlots[i].position = new Vector3(xPos, yPos, 0);
            priceLabels[i].transform.position = new Vector3(xPos, yPos - 1.0f, 0);
            priceLabels[i].alignment = TextAlignmentOptions.Center;
            priceLabels[i].text = ""; // Clear initial text
        }

        Debug.Log("Shop slots and price labels positioned correctly.");
    }

    /// <summary>
    /// Initialize exactly 3 unique shop items with proper scaling and prices.
    /// </summary>
    public void InitializeShopItems()
    {
        List<GameObject> allItems = new List<GameObject>();
        allItems.AddRange(consumablePrefabs);
        allItems.AddRange(powerupPrefabs);
        allItems.AddRange(weaponPrefabs);

        List<GameObject> selectedItems = allItems
            .OrderBy(x => Random.value)
            .Take(3)
            .ToList();

        // Clear existing children from slots
        foreach (var slot in shopSlots)
        {
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }
        }

        // Place one item per slot and assign price
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (i < selectedItems.Count && shopSlots[i] != null)
            {
                GameObject itemPrefab = selectedItems[i];

                GameObject item = Instantiate(itemPrefab, shopSlots[i]);
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = itemPrefab.transform.localScale;

                int randomPrice = DetermineItemPrice(itemPrefab);

                // Only add ShopItem if the object is tagged 'Gun'
                if (item.CompareTag("Gun"))
                {
                    ShopItem shopItem = item.GetComponent<ShopItem>();
                    if (shopItem == null)
                    {
                        shopItem = item.AddComponent<ShopItem>();
                    }

                    shopItem.InitializeItem(randomPrice, itemPrefab, priceLabels[i]);
                }
                else if (item.GetComponent<ShopConsumable>() != null)
                {
                    ShopConsumable shopConsumable = item.GetComponent<ShopConsumable>();
                    shopConsumable.price = randomPrice;
                }

                priceLabels[i].text = $"${randomPrice}";
                priceLabels[i].transform.position = new Vector3(
                    shopSlots[i].position.x,
                    shopSlots[i].position.y - 0.5f,
                    shopSlots[i].position.z
                );

                item.SetActive(true);
                Debug.Log($"Item spawned in Slot {i} with price: ${randomPrice}");
            }
        }

        Debug.Log("3 unique shop items spawned with their original prefab size and correct prices.");
    }

    /// <summary>
    /// Clear all price labels when transitioning rooms.
    /// </summary>
    public void ClearPriceLabels()
    {
        foreach (var label in priceLabels)
        {
            label.text = "";
        }
        Debug.Log("Price labels cleared.");
    }

    /// <summary>
    /// Determines the price based on the item category.
    /// </summary>
    private int DetermineItemPrice(GameObject itemPrefab)
    {
        if (consumablePrefabs.Contains(itemPrefab))
        {
            return Random.Range(consumablePriceRange.x, consumablePriceRange.y + 1);
        }
        else if (powerupPrefabs.Contains(itemPrefab))
        {
            return Random.Range(powerupPriceRange.x, powerupPriceRange.y + 1);
        }
        else if (weaponPrefabs.Contains(itemPrefab))
        {
            return Random.Range(weaponPriceRange.x, weaponPriceRange.y + 1);
        }

        return Random.Range(1, 5); // Default fallback price
    }
}
