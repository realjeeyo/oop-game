using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameObject selectedCharacter;
    public int roomsCompleted = 0; // Tracks the number of rooms completed
    public GameObject gameOverPanel; // Reference to the Game Over UI panel
    public Text statsText; // Reference to the UI Text for stats (ensure this is a Unity UI Text, not TextMeshPro)
    public Button yesButton; // Button to restart the game
    public Button noButton; // Button to quit the game

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure it’s a root object
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }


    public void SetSelectedCharacter(GameObject character)
    {
        selectedCharacter = character;
    }

    public GameObject GetSelectedCharacter()
    {
        return selectedCharacter;
    }

    public void CompleteRoom()
    {
        roomsCompleted++;
    }

    public void TriggerGameOver()
    {
        // Stop the game logic
        Time.timeScale = 0;

        // Show Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            statsText.text = $"Rooms Completed: {roomsCompleted}";
            
            // Disable buttons until the player decides
            yesButton.onClick.RemoveAllListeners(); // Clear any previous listeners
            noButton.onClick.RemoveAllListeners();
            
            yesButton.onClick.AddListener(RestartGame); // Add listener to restart
            noButton.onClick.AddListener(CloseGameOver); // Add listener to close the panel
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void CloseGameOver()
    {
        Time.timeScale = 1; // Resume the game
        SceneManager.LoadScene("SampleScene"); // Replace "MainMenu" with the name of your main menu scene
    }


}
