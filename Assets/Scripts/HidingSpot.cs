using UnityEngine;
using EasyPeasyFirstPersonController;

public class HidingSpot : MonoBehaviour
{
    private bool playerIsInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;
        }
    }

    public bool IsPlayerHiding()
    {
        if (playerIsInside)
        {
            FirstPersonController playerController = FindObjectOfType<FirstPersonController>();
            // This checks if the player is in the hiding spot AND crouching
            return playerController != null && playerController.isCrouching;
        }
        return false;
    }
}
