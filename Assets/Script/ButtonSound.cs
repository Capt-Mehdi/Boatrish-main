using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class PlayAudioButton : MonoBehaviour
{
    // Reference to the audio source (drag and drop from Inspector)
    public AudioSource audioSource;

    // Reference to the audio clip (drag and drop from Inspector)
    public AudioClip audioClip;

    // Reference to the button (drag and drop from Inspector)
    public Button playButton;

    // Time in seconds to play the audio
    public float playTime = 5f;

    void Start()
    {
        // Add event listener to the button's onClick event
        playButton.onClick.AddListener(PlayAudio);
    }

    void PlayAudio()
    {
        // Play the audio clip
        audioSource.Play();

        // Start a coroutine to stop the audio after playTime seconds
        StartCoroutine(StopAudioAfterTime());
    }

    // Coroutine to stop the audio after playTime seconds
    IEnumerator StopAudioAfterTime()
    {
        yield return new WaitForSeconds(playTime);

        // Stop the audio source
        audioSource.Stop();
    }
}