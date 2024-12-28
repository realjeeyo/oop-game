using UnityEngine;
using System.Collections;

public class RoomBoundary : MonoBehaviour
{
    private RoomBasedCamera roomCamera;
    private RoomManager roomManager;
    private EnemySpawner enemySpawner;

    private bool hasTriggeredNextRoom = false; // Prevent repeated triggers
    private bool isCooldownActive = false;    // Prevent quick retriggers

    [Header("Boundary Direction")]
    public Vector3 entryDirection;

    [Header("Cooldown Settings")]
    public float triggerCooldown = 1.0f; // Time before re-triggering is allowed

    private void Start()
    {
        roomCamera = FindObjectOfType<RoomBasedCamera>();
        roomManager = FindObjectOfType<RoomManager>();
        enemySpawner = FindObjectOfType<EnemySpawner>();

        if (roomManager == null)
        {
            Debug.LogError("RoomManager not found in the scene!");
        }

        if (enemySpawner == null)
        {
            Debug.LogWarning("EnemySpawner not found. This might not be an EnemyRoom or BossRoom.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || hasTriggeredNextRoom || isCooldownActive)
            return; // Prevent invalid triggers or cooldown retriggers

        StartCoroutine(TriggerCooldown());

        if (AreEnemiesCleared())
        {
            hasTriggeredNextRoom = true; // Prevent multiple triggers

            if (roomManager != null)
            {
                roomManager.GenerateRoom(transform.position, entryDirection);
                Debug.Log("Boundary triggered: Moving to the next room.");
            }

            Destroy(gameObject, 0.5f); // Destroy boundary safely after transition
        }
        else
        {
            Debug.Log("Cannot proceed: Enemies or Boss are still alive!");
        }
    }

    /// <summary>
    /// Checks if all enemies in the room are defeated.
    /// </summary>
    private bool AreEnemiesCleared()
    {
        if (enemySpawner != null && enemySpawner.HasSpawned && enemySpawner.ActiveEnemyCount == 0)
        {
            Debug.Log("All enemies defeated. Boundary is now unlocked.");
            return true;
        }
        else
        {
            Debug.Log("Enemies are still alive. Boundary remains locked.");
            return false;
        }
    }

    /// <summary>
    /// Activates a cooldown to prevent repeated triggers.
    /// </summary>
    private IEnumerator TriggerCooldown()
    {
        isCooldownActive = true;
        yield return new WaitForSeconds(triggerCooldown);
        isCooldownActive = false;
    }
}
