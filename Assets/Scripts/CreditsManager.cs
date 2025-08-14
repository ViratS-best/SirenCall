using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip winningAudioClip;

    void Start()
    {
        // --- Play the winning audio on start and loop it ---
        if (audioSource != null && winningAudioClip != null)
        {
            audioSource.clip = winningAudioClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void GoToMainMenu()
    {
        // --- Stop the audio and go back to the Main Menu ---
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        SceneManager.LoadScene("MainMenu");
    }
}