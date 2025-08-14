using UnityEngine;
using UnityEngine.SceneManagement;
using CodeMonkey.KeyDoorSystemCM; // <-- Add this line
using EasyPeasyFirstPersonController;

public class WinCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the EnemyAI and stop the chase
            EnemyAI enemyAI = FindObjectOfType<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.currentState = AIState.Disabled;
                
                // Stop the chase music on the player
                if (enemyAI.playerAudioSource != null)
                {
                    enemyAI.playerAudioSource.Stop();
                }
            }

            // --- NEW: Unlock and show the mouse cursor ---
            FirstPersonController playerController = other.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                playerController.SetCursorVisibility(true);
            }
            // ---------------------------------------------
            
            // Load the Credits scene
            SceneManager.LoadScene("CreditsScene");
        }
    }
}