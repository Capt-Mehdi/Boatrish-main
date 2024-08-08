using UnityEngine;

public class PlayButtonSound : MonoBehaviour
{
    public AudioSource buttonSound;

    void Start()
    {
        if (buttonSound == null)
        {
            buttonSound = GetComponent<AudioSource>();
        }
    }

    public void PlaySound()
    {
        if (buttonSound != null && !buttonSound.isPlaying)
        {
            buttonSound.Play();
        }
    }
}
