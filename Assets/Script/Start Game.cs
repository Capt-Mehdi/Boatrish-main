using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    // Reference to the play button
    public Button playButton;

    void Start()
    {
        // Ensure the button is assigned
        if (playButton != null)
        {
            // Add a listener to the button to call the StartGame method when clicked
            playButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Play button is not assigned in the inspector.");
        }
    }

    // Method to start the game by loading the first scene
    void StartGame()
    {
        // Load the scene named "scene_1"
        SceneManager.LoadScene("Level_1");
    }
}
