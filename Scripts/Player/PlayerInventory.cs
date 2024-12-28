using UnityEngine;
using TMPro;

public interface IInventoryManager
{
    void AddCoins(int amount);
    void UpdateCoinUI();
}

public class PlayerInventory : MonoBehaviour, IInventoryManager
{
    public static PlayerInventory Instance { get; private set; }

    public int coinCount = 0;
    [SerializeField] private TextMeshProUGUI coinText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddCoins(int amount)
    {
        if (amount < 0)
            throw new System.ArgumentException("Cannot add a negative amount of coins.");

        coinCount += amount;
        Debug.Log($"Coins: {coinCount}");
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {coinCount}";
        }
        else
        {
            Debug.LogWarning("CoinText UI is not assigned in the Inspector.");
        }
    }
}
