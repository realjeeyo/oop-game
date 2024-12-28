using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs; // Enemy prefabs for EnemyRoom
    public GameObject bossPrefab; // Boss prefab for BossRoom
    public int spawnCount = 3; // Base number of enemies in EnemyRoom
    public float spawnDelay = 1f; // Delay before spawning starts

    [Header("Spawner Settings")]
    public float margin = 1f; // Margin to prevent spawning outside camera bounds

    public bool HasSpawned { get; private set; } = false; // Prevent multiple spawns

    private List<BaseEnemy> activeEnemies = new List<BaseEnemy>(); // Track active enemies
    private RoomBasedCamera roomCamera; // For camera bounds
    private int difficultyLevel = 0; // Room-specific difficulty level

    // Static Difficulty Settings (Global scaling from boss kills)
    public static int BossKills = 0;        // Tracks number of boss kills
    public static int ExtraEnemyCount = 0;  // +1 enemy per boss kill
    public static int HealthBonus = 0;      // +1 health per boss kill
    public static int DamageBonus = 0;      // +1 damage every 3 boss kills (capped at 2)

    public int ActiveEnemyCount => activeEnemies.Count;

    private void Start()
    {
        roomCamera = FindObjectOfType<RoomBasedCamera>();
        if (roomCamera == null)
        {
            Debug.LogError("RoomBasedCamera not found in the scene!");
        }
    }

    /// <summary>
    /// Set the difficulty level for the current room.
    /// </summary>
    public void SetDifficultyLevel(int level)
    {
        difficultyLevel = level;
        Debug.Log($"Spawner difficulty level set to: {difficultyLevel}");
    }

    /// <summary>
    /// Trigger enemy or boss spawning based on room type.
    /// </summary>
    public void SpawnEnemiesWithDelay()
    {
        if (HasSpawned)
        {
            Debug.LogWarning("Enemies or Boss have already been spawned in this room.");
            return;
        }

        if (transform.parent.CompareTag("BossRoom"))
        {
            StartCoroutine(SpawnBossAfterDelay());
        }
        else if (transform.parent.CompareTag("EnemyRoom"))
        {
            StartCoroutine(SpawnEnemiesAfterDelay());
        }
        else
        {
            Debug.LogWarning("EnemySpawner is not in a recognized room type (EnemyRoom or BossRoom).");
        }
    }

    /// <summary>
    /// Spawns regular enemies for EnemyRoom.
    /// </summary>
    private IEnumerator SpawnEnemiesAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        Bounds cameraBounds = roomCamera.GetCameraBounds();

        float leftBound = cameraBounds.min.x + margin;
        float rightBound = cameraBounds.max.x - margin;
        float bottomBound = cameraBounds.min.y + margin;
        float topBound = cameraBounds.max.y - margin;

        int totalEnemies = spawnCount + difficultyLevel + ExtraEnemyCount;

        for (int i = 0; i < totalEnemies; i++)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(leftBound, rightBound),
                Random.Range(bottomBound, topBound)
            );

            GameObject newEnemyObj = Instantiate(
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                spawnPosition,
                Quaternion.identity
            );

            BaseEnemy newEnemy = newEnemyObj.GetComponent<BaseEnemy>();
            if (newEnemy != null)
            {
                // Apply global difficulty scaling
                newEnemy.health += HealthBonus;
                newEnemy.damage += DamageBonus;

                activeEnemies.Add(newEnemy);
                newEnemyObj.SetActive(true);
                newEnemy.OnDeath += () => RemoveEnemy(newEnemy);
            }
        }

        HasSpawned = true;
        Debug.Log($"Spawned {totalEnemies} enemies with HealthBonus: {HealthBonus}, DamageBonus: {DamageBonus}");
    }

    /// <summary>
    /// Spawns a single boss enemy for BossRoom.
    /// </summary>
    private IEnumerator SpawnBossAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        Bounds cameraBounds = roomCamera.GetCameraBounds();
        Vector3 bossSpawnPosition = new Vector3(
            cameraBounds.center.x,
            cameraBounds.center.y,
            0f
        );

        if (bossPrefab != null)
        {
            GameObject bossObj = Instantiate(bossPrefab, bossSpawnPosition, Quaternion.identity);
            bossObj.SetActive(true);

            BossEnemy boss = bossObj.GetComponent<BossEnemy>();
            if (boss != null)
            {
                // Scale boss stats based on difficulty level and boss kills
                boss.ScaleStrength(1 + difficultyLevel * 0.5f);
                activeEnemies.Add(boss);

                Debug.Log($"Spawned Boss at position: {bossSpawnPosition}");
            }
        }
        else
        {
            Debug.LogError("Boss prefab is not assigned in EnemySpawner!");
        }

        HasSpawned = true;
    }

    /// <summary>
    /// Remove enemy or boss from active list upon death.
    /// </summary>
    public void RemoveEnemy(BaseEnemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"Enemy removed. Active enemies remaining: {ActiveEnemyCount}");
        }
        else
        {
            Debug.LogWarning($"Attempted to remove an enemy not in the active list: {enemy.gameObject.name}");
        }

        if (ActiveEnemyCount == 0)
        {
            Debug.Log("All enemies or boss defeated in this room. Room is now cleared.");
        }
    }

    /// <summary>
    /// Increase global difficulty when a boss is defeated.
    /// </summary>
    public static void IncreaseGlobalDifficulty()
    {
        BossKills++;
        ExtraEnemyCount++;
        HealthBonus++;

        if (BossKills % 3 == 0 && DamageBonus < 2)
        {
            DamageBonus++;
        }

        Debug.Log($"Global Difficulty Increased: BossKills={BossKills}, ExtraEnemies={ExtraEnemyCount}, HealthBonus={HealthBonus}, DamageBonus={DamageBonus}");
    }
}
