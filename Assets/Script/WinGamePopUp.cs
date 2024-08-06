using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinGamePopup : MonoBehaviour
{
    public GameObject popupPanel;
    public Button nextLevelButton;
    public Button mainMenuButton;
    private bool hasWon = false;

    void Start()
    {
        // Ensure the popup panel is hidden at the start
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Popup Panel is not assigned in the Inspector.");
        }

        // Add listeners to the buttons
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(NextLevel);
        }
        else
        {
            Debug.LogError("Next Level Button is not assigned in the Inspector.");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(MainMenu);
        }
        else
        {
            Debug.LogError("Main Menu Button is not assigned in the Inspector.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WinGame") && !hasWon)
        {
            hasWon = true;
            // Show the pop-up
            if (popupPanel != null)
            {
                popupPanel.SetActive(true);
                Debug.Log("Popup Panel is now active.");
            }
            else
            {
                Debug.LogError("Popup Panel is not assigned.");
            }

            // Pause the game or stop boat movement if necessary
            // Time.timeScale = 0; // Uncomment to pause the game
        }
        else
        {
            Debug.Log("Collision detected with object that does not have the WinGame tag or hasWon is already true.");
        }
    }

    void NextLevel()
    {
        // Load the next level (assuming your levels are named sequentially)
        // Adjust this to your actual next level name or logic
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void MainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}
