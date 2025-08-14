using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using EasyPeasyFirstPersonController;
using CodeMonkey.KeyDoorSystemCM;
using System.Collections;

public enum AIState { Patrol, Chase, Alerted, Disabled }

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 6f;
    public float alertedSpeed = 8f;
    public float killRange = 2f;
    public float chaseRange = 5f;
    public float hidingKillRange = 2.5f;
    
    public string jumpscareAnimationName = "Zombie Neck Bite"; 
    public Light jumpscareLight;
    public Transform[] waypoints;
    
    // --- NEW AUDIO VARIABLES ---
    public AudioSource enemyAudioSource;
    public AudioSource playerAudioSource;

    public AudioClip jumpscareAudio;
    public AudioClip stompingAudio;
    public AudioClip chaseAudio;
    // -------------------------

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    public AIState currentState = AIState.Patrol;
    private Animator animator;

    private FirstPersonController playerController;
    private Camera playerCamera;
    private float shakeDuration = 0.3f;
    private float shakeAmount = 0.5f;
    private Quaternion originalCamRotation;
    
    public Transform jumpscareLookTarget; 
    public Transform jumpscareCameraPosition; 
    public float cameraDampening = 5f;

    private DoorKeyHolder playerKeyHolder;
    private bool hasKeyBeenCollected = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        playerController = player.GetComponent<FirstPersonController>();
        playerCamera = playerController.playerCamera.GetComponent<Camera>();
        originalCamRotation = playerCamera.transform.localRotation;
        
        playerKeyHolder = player.GetComponent<DoorKeyHolder>();

        if (playerKeyHolder != null)
        {
            playerKeyHolder.OnDoorKeyAdded += OnKeyCollected;
        }

        agent.speed = patrolSpeed;
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
    
    private void OnKeyCollected(object sender, System.EventArgs e)
    {
        hasKeyBeenCollected = true;
        
        // --- NEW: Play chase audio when key is collected ---
        if (playerAudioSource != null && chaseAudio != null)
        {
            playerAudioSource.clip = chaseAudio;
            playerAudioSource.loop = true;
            playerAudioSource.Play();
        }
        // --------------------------------------------------
        
        StartChase(); 
    }

    void Update()
    {
        bool isPlayerHiding = false;
        HidingSpot[] hidingSpots = FindObjectsOfType<HidingSpot>();
        foreach (HidingSpot spot in hidingSpots)
        {
            if (spot.IsPlayerHiding())
            {
                isPlayerHiding = true;
                break;
            }
        }
        
        if (!isPlayerHiding && (hasKeyBeenCollected || Vector3.Distance(transform.position, player.position) <= chaseRange) && currentState != AIState.Chase)
        {
            StartChase();
        }
        
        // --- NEW: Audio logic based on distance and hiding ---
        if (isPlayerHiding && Vector3.Distance(transform.position, player.position) < 5f)
        {
            if (enemyAudioSource != null && stompingAudio != null && !enemyAudioSource.isPlaying)
            {
                enemyAudioSource.clip = stompingAudio;
                enemyAudioSource.loop = true;
                enemyAudioSource.Play();
            }
        }
        else
        {
            if (enemyAudioSource != null && enemyAudioSource.isPlaying)
            {
                enemyAudioSource.Stop();
            }
        }
        // ----------------------------------------------------

        animator.SetFloat("Speed", agent.velocity.magnitude);

        switch (currentState)
        {
            case AIState.Patrol:
                agent.speed = patrolSpeed;
                if (waypoints.Length > 0 && agent.remainingDistance < 1f && !agent.pathPending)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                    agent.SetDestination(waypoints[currentWaypointIndex].position);
                }
                break;

            case AIState.Chase:
                if (isPlayerHiding)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                }
                break;

            case AIState.Alerted:
                agent.speed = alertedSpeed;
                agent.SetDestination(player.position);
                break;

            case AIState.Disabled:
                agent.isStopped = true;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Vector3.Distance(transform.position, player.position) <= killRange)
        {
            KillPlayer();
        }
    }

    public void StartChase()
    {
        currentState = AIState.Chase;
        agent.isStopped = false;
    }

    private void ReturnToPatrol()
    {
        currentState = AIState.Patrol;
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
    
    public void GetAlerted()
    {
        currentState = AIState.Alerted;
        Invoke("ReturnToChaseState", 5f);
    }

    private void ReturnToChaseState()
    {
        if (currentState == AIState.Alerted)
        {
            currentState = AIState.Chase;
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Player has been caught!");
        currentState = AIState.Disabled;
        
        agent.isStopped = true;
        playerController.enabled = false;
        
        if (jumpscareLookTarget != null)
        {
            playerCamera.transform.position = jumpscareCameraPosition.position;
            StartCoroutine(JumpscareCameraStabilization());
        }
        
        if (jumpscareLight != null)
        {
            jumpscareLight.enabled = true;
        }
        
        // --- NEW: Play jumpscare audio ---
        if (playerAudioSource != null && jumpscareAudio != null)
        {
            playerAudioSource.Stop(); // Stop chase music
            playerAudioSource.PlayOneShot(jumpscareAudio);
        }
        // ----------------------------------
        
        animator.Play(jumpscareAnimationName);
        StartCoroutine(CameraShake());
        
        // Always restart the scene
        StartCoroutine(WaitAndResetScene(3f));
    }
    
    // --- Removed old Respawn coroutine for simplicity ---
    private IEnumerator WaitAndResetScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private IEnumerator CameraShake()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < shakeDuration)
        {
            Vector3 shakePos = Random.insideUnitSphere * shakeAmount;
            playerCamera.transform.localPosition = new Vector3(shakePos.x, shakePos.y, playerCamera.transform.localPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerCamera.transform.localRotation = originalCamRotation;
    }
    
    private IEnumerator JumpscareCameraStabilization()
    {
        while (true)
        {
            if (jumpscareLookTarget != null)
            {
                Vector3 lookDirection = jumpscareLookTarget.position - playerCamera.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, Time.deltaTime * cameraDampening);
            }
            yield return null;
        }
    }
}