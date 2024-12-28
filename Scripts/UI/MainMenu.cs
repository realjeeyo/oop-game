using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject characterSelectionPanel; // Assign in Inspector
    public GameObject ControlPanel;

    public void PlayGame()
    {
        // Show Character Selection
        characterSelectionPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ShowControls()
    {
        ControlPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        // This stops play mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // This quits the application in a standalone build
        Application.Quit();
    #endif
    }
}
