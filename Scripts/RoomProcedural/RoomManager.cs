using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public GameObject[] roomPrefabs; // EnemyRoom, ShopRoom, BossRoom
    public Transform player; // Player reference

    private Vector3 lastRoomPosition = Vector3.zero;
    private int currentRoomCount = 0;
    private int difficultyLevel = 0;

    private RoomBasedCamera roomCamera;
    private bool isRoomTransitioning = false;

    private Queue<GameObject> roomHistory = new Queue<GameObject>(); // Track previous rooms
    private const int maxRoomHistory = 2; // Number of rooms to keep before cleaning up

    private void Start()
    {
        roomCamera = FindObjectOfType<RoomBasedCamera>();
        StartCoroutine(WaitForPlayerAndInitialize());
    }

    private IEnumerator WaitForPlayerAndInitialize()
    {
        while (player == null)
        {
            player = FindObjectOfType<PlayerController>()?.transform;
            yield return null;
        }

        GenerateFirstRoom();
    }

    public void GenerateFirstRoom()
    {
        if (roomCamera == null)
        {
            roomCamera = FindObjectOfType<RoomBasedCamera>();
        }

        Bounds cameraBounds = roomCamera.GetCameraBounds();
        Vector3 firstRoomPosition = cameraBounds.center;
        firstRoomPosition.z = 0f;

        lastRoomPosition = firstRoomPosition;

        GameObject firstRoom = Instantiate(
            roomPrefabs.FirstOrDefault(prefab => prefab.CompareTag("EnemyRoom")),
            firstRoomPosition,
            Quaternion.identity
        );

        firstRoom.SetActive(true);

        // Add first room to the room history
        AddRoomToHistory(firstRoom);

        PlaceRoomBoundary(firstRoom, cameraBounds);

        Transform playerSpawn = firstRoom.transform.Find("PlayerSpawn");
        if (playerSpawn != null)
        {
            player.position = new Vector3(playerSpawn.position.x, playerSpawn.position.y, 0f);
        }
        else
        {
            player.position = new Vector3(cameraBounds.min.x + 1f, cameraBounds.center.y, 0f);
        }

        EnemySpawner spawner = firstRoom.GetComponentInChildren<EnemySpawner>();
        if (spawner != null)
        {
            spawner.SetDifficultyLevel(difficultyLevel);
            spawner.SpawnEnemiesWithDelay();
            Debug.Log("First EnemyRoom initialized with enemies.");
        }

        Debug.Log($"First room spawned at: {firstRoomPosition}, Player positioned at: {player.position}");
    }

    public void GenerateRoom(Vector3 boundaryPosition, Vector3 entryDirection)
    {
        if (isRoomTransitioning)
        {
            Debug.Log("Room transition is already in progress. Ignoring further calls.");
            return;
        }

        if (!AreEnemiesCleared())
        {
            Debug.Log("Cannot proceed to the next room. Enemies or Boss are still alive!");
            return;
        }

        StartCoroutine(HandleRoomTransition(entryDirection));
    }

    private bool AreEnemiesCleared()
    {
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null && spawner.HasSpawned && spawner.ActiveEnemyCount == 0)
        {
            Debug.Log("All enemies or boss defeated. Room can transition.");
            return true;
        }
        return false;
    }

    private IEnumerator HandleRoomTransition(Vector3 entryDirection)
    {
        isRoomTransitioning = true;

        if (roomCamera == null)
        {
            Debug.LogError("RoomBasedCamera not found!");
            yield break;
        }

        Bounds cameraBounds = roomCamera.GetCameraBounds();
        Vector3 newRoomPosition = lastRoomPosition + (entryDirection * cameraBounds.size.x);
        newRoomPosition.z = 0f;

        Vector3 targetCameraPosition = cameraBounds.center + (entryDirection * cameraBounds.size.x);
        roomCamera.MoveToPosition(targetCameraPosition);

        float timer = 0f;
        float maxWaitTime = 3f;
        while (roomCamera.IsMoving && timer < maxWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        lastRoomPosition = newRoomPosition;
        GameObject nextRoomPrefab = GetRoomPrefab();
        GameObject nextRoom = Instantiate(nextRoomPrefab, newRoomPosition, Quaternion.identity);
        nextRoom.SetActive(true);

        AddRoomToHistory(nextRoom); // Track the new room

        PlaceRoomBoundary(nextRoom, roomCamera.GetCameraBounds());

        player.position = new Vector3(
            roomCamera.GetCameraBounds().min.x + 1f,
            roomCamera.GetCameraBounds().center.y,
            0f
        );

        if (nextRoom.CompareTag("ShopRoom"))
        {
            ShopManager shopManager = nextRoom.GetComponentInChildren<ShopManager>();
            if (shopManager != null)
            {
                shopManager.InitializeShopItems();
                Debug.Log("ShopRoom initialized with shop items.");
            }
        }
        else
        {
            ShopManager shopManager = FindObjectOfType<ShopManager>();
            if (shopManager != null)
            {
                shopManager.ClearPriceLabels();
                Debug.Log("Cleared price labels because this is not a ShopRoom.");
            }
        }

        EnemySpawner spawner = nextRoom.GetComponentInChildren<EnemySpawner>();
        if (spawner != null)
        {
            spawner.SetDifficultyLevel(difficultyLevel);
            spawner.SpawnEnemiesWithDelay();
            Debug.Log($"{nextRoom.tag} initialized with enemies or boss.");
        }

        currentRoomCount++;
        GameManager.Instance.CompleteRoom();
        isRoomTransitioning = false;
    }

    private GameObject GetRoomPrefab()
    {
        if (currentRoomCount % 3 == 0)
        {
            return roomPrefabs.FirstOrDefault(prefab => prefab.CompareTag("ShopRoom"));
        }
        else if (currentRoomCount % 5 == 0)
        {
            return roomPrefabs.FirstOrDefault(prefab => prefab.CompareTag("BossRoom"));
        }
        return roomPrefabs.FirstOrDefault(prefab => prefab.CompareTag("EnemyRoom"));
    }

    private void AddRoomToHistory(GameObject room)
    {
        roomHistory.Enqueue(room);

        if (roomHistory.Count > maxRoomHistory)
        {
            GameObject oldRoom = roomHistory.Dequeue();
            if (oldRoom != null)
            {
                Destroy(oldRoom);
                Debug.Log($"Destroyed old room: {oldRoom.name}");
            }
        }
    }

    private void PlaceRoomBoundary(GameObject room, Bounds cameraBounds)
    {
        Vector3 boundaryPosition = new Vector3(
            cameraBounds.max.x + 0.5f,
            cameraBounds.center.y,
            0f
        );

        GameObject boundary = new GameObject("RightBoundary");
        boundary.transform.position = boundaryPosition;
        boundary.transform.parent = room.transform;

        BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1, cameraBounds.size.y);

        Rigidbody2D rb = boundary.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        RoomBoundary roomBoundary = boundary.AddComponent<RoomBoundary>();
        roomBoundary.entryDirection = Vector3.right;

        Debug.Log($"Boundary placed on RIGHT side at: {boundaryPosition}");
    }
}
