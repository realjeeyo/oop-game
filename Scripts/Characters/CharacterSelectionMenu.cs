using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionMenu : MonoBehaviour
{
    public GameObject characterSelectionPanel;  // Reference to the character selection panel
    public GameObject phoenixPrefab;
    public GameObject jettPrefab;

    public Transform spawnPoint;  // Reference to the spawn point where the character will appear

    public void SelectPhoenix()
    {
        GameObject phoenix = Instantiate(phoenixPrefab, spawnPoint.position, spawnPoint.rotation);
        phoenix.SetActive(true);
        phoenix.tag = "Player";

        // Assign the cooldown UI and set the ability icon
        CooldownUIManager cooldownUI = FindObjectOfType<CooldownUIManager>();
        
        cooldownUI.SetAbilityIcon("Phoenix");

        characterSelectionPanel.SetActive(false);
    }

    public void SelectJett()
    {
        GameObject jett = Instantiate(jettPrefab, spawnPoint.position, spawnPoint.rotation);
        jett.SetActive(true);
        jett.tag = "Player";

        // Assign the cooldown UI and set the ability icon
        CooldownUIManager cooldownUI = FindObjectOfType<CooldownUIManager>();
  
        cooldownUI.SetAbilityIcon("Jett");

        characterSelectionPanel.SetActive(false);
    }


    public void StartGame()
    {
        // Start the game or transition to the next scene
        SceneManager.LoadScene("GameScene");  // Change this to your actual game scene name
    }
}
