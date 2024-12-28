using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public Transform spawnPoint; // Define the spawn point in your scene

    void Start()
    {
        GameObject selectedCharacter = GameManager.Instance.GetSelectedCharacter();
        if (selectedCharacter != null)
        {
            Instantiate(selectedCharacter, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No character selected! Returning to menu.");
            // Optionally reload the menu scene
        }
    }
}
