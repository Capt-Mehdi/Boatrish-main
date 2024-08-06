using UnityEngine;

public class EndGame : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Check if the boat entered the trigger
        if (other.gameObject.CompareTag("Boat"))
        {
            // Find and disable the boat's movement script
            BoatMovement boatMovement = other.gameObject.GetComponent<BoatMovement>();
            if (boatMovement != null)
            {
                boatMovement.enabled = false;
            }

            // Optionally, you can display a game over message or take other actions here
            Debug.Log("Game Over! The boat has reached the end of the river.");
        }
    }
}
