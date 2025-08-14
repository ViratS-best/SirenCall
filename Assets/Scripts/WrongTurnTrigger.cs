using UnityEngine;
using CodeMonkey.KeyDoorSystemCM; // <-- Add this line

public class WrongTurnTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Only alert the monster if the key has been collected
            if (DoorKey.keyCollected) // <-- Access the static variable directly
            {
                EnemyAI monsterAI = FindObjectOfType<EnemyAI>();
                if (monsterAI != null && monsterAI.currentState == AIState.Chase)
                {
                    monsterAI.GetAlerted();
                }
            }
        }
    }
}