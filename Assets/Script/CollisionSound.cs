using UnityEngine;

public class BoatCollisionSound : MonoBehaviour
{
    // The audio source component
    public AudioSource audioSource;

    // The sound to play on collision
    public AudioClip collisionSound;

    // The volume of the sound
    public float volume = 1.0f;

    // Called when the boat collides with another object
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with an obstacle
        if (collision.gameObject.tag == "Obstacle")
        {
            // Play the collision sound
            audioSource.PlayOneShot(collisionSound, volume);
        }
    }
}