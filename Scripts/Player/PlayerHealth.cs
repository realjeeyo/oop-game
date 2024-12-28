using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    private int maxHealth = 3;
    private int currentHealth;

    private int maxShields = 2;
    private int currentShields = 0;

    public GameObject heartPrefab;
    public GameObject shieldIconPrefab;

    public Transform heartsPanel;
    public Transform shieldsPanel;

    private List<GameObject> heartIcons = new List<GameObject>();
    private List<GameObject> shieldIcons = new List<GameObject>();

    // Public getters and setters
    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = Mathf.Max(value, 0); // Ensure max health is non-negative
            UpdateUI();
        }
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            UpdateUI();
            CheckGameOver();
        }
    }

    void CheckGameOver()
    {
        if (CurrentHealth <= 0)
        {
            Debug.Log("Game Over! Restarting game...");

            // Add a null check for GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver(); // Show Game Over screen
            }
            else
            {
                Debug.LogError("GameManager instance is not initialized!");
            }
        }
    }




    public int MaxShields
    {
        get => maxShields;
        set
        {
            maxShields = Mathf.Max(value, 0); // Ensure max shields are non-negative
            UpdateUI();
        }
    }

    public int CurrentShields
    {
        get => currentShields;
        set
        {
            currentShields = Mathf.Clamp(value, 0, maxShields);
            UpdateUI();
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentShields > 0) RemoveShield();
        else CurrentHealth -= damage;
    }

    public void AddHealth()
    {
        MaxHealth += 1;
        CurrentHealth += 1;
    }

    public void AddShield()
    {
        MaxShields += 1;
        CurrentShields += 1;
    }

    public void ReplenishHealth()
    {
        CurrentHealth += 1;
    }

    public void ReplenishShield()
    {
        CurrentShields += 1;
    }

    private void RemoveShield()
    {
        CurrentShields--;
    }

    private void UpdateUI()
    {
        foreach (GameObject heart in heartIcons) Destroy(heart);
        heartIcons.Clear();
        foreach (GameObject shield in shieldIcons) Destroy(shield);
        shieldIcons.Clear();

        for (int i = 0; i < currentHealth; i++)
        {
            if (heartPrefab != null)
            {
                GameObject newHeart = Instantiate(heartPrefab, heartsPanel);
                heartIcons.Add(newHeart);
            }
        }

        for (int i = 0; i < currentShields; i++)
        {
            if (shieldIconPrefab != null)
            {
                GameObject newShield = Instantiate(shieldIconPrefab, shieldsPanel);
                shieldIcons.Add(newShield);
            }
            else
            {
                Debug.LogError("Shield Icon Prefab is not assigned in PlayerHealth.");
            }
        }
    }
}
