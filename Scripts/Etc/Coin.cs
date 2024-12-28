using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1; // Base value of the coin

    // Allows setting a custom value for the coin
    public void SetValue(int newValue)
    {
        value = newValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Add coins to the player's inventory
            PlayerInventory.Instance?.AddCoins(value);

            // Destroy the coin after collection
            Destroy(gameObject);
        }
    }
}
