using UnityEngine;
using System;

public abstract class BaseEnemy : MonoBehaviour
{
    public int health = 10;
    public int damage = 1;
    public float moveSpeed = 2f;
    public float activationDelay = 2f; // Delay before enemy starts acting

    protected Transform player;
    protected bool isActive = false;
    protected bool isDead = false; // Flag to prevent multiple death triggers

    public GameObject coinPrefab; // Assign this in the Unity Editor
    protected EnemySpawner enemySpawner; // Reference to the enemy spawner

    public event Action OnDeath;

    private Vector2 lastPosition; // To track movement direction

    // 🔄 Contact Damage Cooldown
    private float lastContactTime = -1.5f; // Initialize to ensure immediate damage on first contact
    private float contactCooldown = 1.5f; // 1.5 seconds cooldown between damage on contact

    protected void TriggerOnDeath()
    {
        OnDeath?.Invoke();
    }

    protected virtual void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>(); // Get the EnemySpawner in the scene
        lastPosition = transform.position; // Initialize last position

        StartCoroutine(ActivateAfterDelay());
    }

    protected virtual void Update()
    {
        if (isActive && player != null)
        {
            MoveTowardsPlayer();
        }

        FlipSpriteBasedOnDirection();
    }

    protected void MoveTowardsPlayer()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void FlipSpriteBasedOnDirection()
    {
        Vector2 currentPosition = transform.position;
        float directionX = currentPosition.x - lastPosition.x;

        if (directionX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (directionX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        lastPosition = currentPosition;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        DropCoin();

        OnDeath?.Invoke();

        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(this);
        }

        Destroy(gameObject);
    }

    protected void DropCoin()
    {
        if (coinPrefab != null)
        {
            GameObject coinInstance = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            coinInstance.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Coin prefab is not assigned in the inspector!");
        }
    }

    public abstract void Attack();

    private System.Collections.IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        isActive = true;
        StartCoroutine(FindPlayer());
    }

    private System.Collections.IEnumerator FindPlayer()
    {
        while (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    // 🛡️ Handle Contact Damage with Cooldown
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float currentTime = Time.time;

            if (currentTime - lastContactTime >= contactCooldown)
            {
                // Apply damage
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    lastContactTime = currentTime; // Reset cooldown timer
                    Debug.Log($"Player damaged by {gameObject.name}. Next damage allowed after {contactCooldown} seconds.");
                }
            }
        }
    }
}
