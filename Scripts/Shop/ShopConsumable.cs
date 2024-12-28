using UnityEngine;

public class ShopConsumable : MonoBehaviour
{
    public enum ConsumableType
    {
        Heart,
        Shield,
        HealthPotion,
        ShieldPotion,
        DamageBoost,
        AttackSpeedBoost,
        MoveSpeedBoost
    }

    [Header("Consumable Settings")]
    public ConsumableType itemType;
    public int price = 10;
    public float boostAmount = 0.05f; // Smaller, permanent boost amount for balance

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInventory inventory = PlayerInventory.Instance;
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            PlayerStatsModifier statsModifier = other.GetComponent<PlayerStatsModifier>();

            if (inventory.coinCount < price)
            {
                Debug.Log("Not enough coins!");
                return;
            }

            inventory.coinCount -= price;
            inventory.UpdateCoinUI();

            switch (itemType)
            {
                case ConsumableType.Heart:
                    playerHealth?.AddHealth();
                    Debug.Log("Heart gained!");
                    break;

                case ConsumableType.Shield:
                    playerHealth?.AddShield();
                    Debug.Log("Shield gained!");
                    break;

                case ConsumableType.HealthPotion:
                    playerHealth?.ReplenishHealth();
                    Debug.Log("Health replenished!");
                    break;

                case ConsumableType.ShieldPotion:
                    playerHealth?.ReplenishShield();
                    Debug.Log("Shields replenished!");
                    break;

                case ConsumableType.DamageBoost:
                    statsModifier?.ApplyPermanentDamageMultiplier(1 + boostAmount);
                    Debug.Log($"Permanent Damage Boost applied: +{boostAmount * 100}%");
                    break;

                case ConsumableType.AttackSpeedBoost:
                    statsModifier?.ApplyPermanentAttackSpeedBoost(1 + boostAmount);
                    Debug.Log($"Permanent Attack Speed Boost applied: +{boostAmount * 100}%");
                    break;

                case ConsumableType.MoveSpeedBoost:
                    statsModifier?.ApplyPermanentMoveSpeedBoost(1 + boostAmount);
                    Debug.Log($"Permanent Move Speed Boost applied: +{boostAmount * 100}%");
                    break;
            }

            Destroy(gameObject); // Remove the consumable after purchase
        }
    }
}
