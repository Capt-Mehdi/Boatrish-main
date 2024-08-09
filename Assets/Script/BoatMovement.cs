using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BoatMovement : MonoBehaviour
{
    // Camera and Animation
    public Camera mainCamera;
    public Camera winCamera;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    public Animator playerAnim;
    public GameObject player;

    // Buoyancy and physics settings
    public float waterLevel = 0.3f;
    public float floatHeight = 0.5f;
    public float buoyancyStrength = 20f;
    public float waterDrag = 0.5f;
    public float rotationSpeed = 5f;
    public float rotationResetDuration = 10f;
    public PhysicMaterial zeroFrictionMaterial;

    // UI Elements
    public GameObject winPopupPanel;
    public GameObject obstaclePopupPanel;
    public Button nextLevelButton;
    public Button mainMenuButton;
    public Button retryButton;

    public GameObject endZonePopupPanel;
    public GameObject obstacleRetryPopupPanel;
    public Button endZoneRetryButton;
    public Button endZoneHomeButton;
    public Button obstacleRetryButton;
    public Button obstacleHomeButton;

    // Audio Clips
    public AudioClip winAudioClip;
    public AudioClip loseAudioClip;
    public AudioClip endZoneAudioClip;
    public AudioSource winAudioSource;
    public AudioSource loseAudioSource;
    public AudioSource endZoneAudioSource;

    private Rigidbody rb;
    private bool hasWonOrDestroyed = false;
    private bool collisionDetected = false;
    private bool isResettingRotation = false;
    private float rotationResetStartTime;
    private Quaternion originalRotation;
    private int lineCollisionCount = 0;
    private Collider boatCollider;
    private Vector3 currentDirection;

    private Vector3 previousPosition;
    private float timeSinceLastMove = 0f;
    public float maxStuckTime = 5f; // Maximum time allowed to be stuck in seconds

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.angularDrag = 10f;

        // Ensure AudioSource components are present
        if (winAudioSource == null)
        {
            winAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (loseAudioSource == null)
        {
            loseAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (endZoneAudioSource == null)
        {
            endZoneAudioSource = gameObject.AddComponent<AudioSource>();
        }

        originalRotation = transform.rotation;

        boatCollider = GetComponent<Collider>();
        if (boatCollider != null && zeroFrictionMaterial != null)
        {
            boatCollider.material = zeroFrictionMaterial;
        }

        currentDirection = transform.forward;

        InitializeUI(winPopupPanel, nextLevelButton, NextLevel);
        InitializeUI(winPopupPanel, mainMenuButton, MainMenu);
        InitializeUI(winPopupPanel, retryButton, RestartLevel);
        InitializeUI(endZonePopupPanel, endZoneRetryButton, RestartLevel);
        InitializeUI(endZonePopupPanel, endZoneHomeButton, MainMenu);
        InitializeUI(obstacleRetryPopupPanel, obstacleRetryButton, RestartLevel);
        InitializeUI(obstacleRetryPopupPanel, obstacleHomeButton, MainMenu);

        previousPosition = transform.position;

        if (player != null)
        {
            playerAnim = player.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned in the BoatMovement script.");
        }

        // Save the original camera position and rotation
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
        }

        // Ensure the win camera is disabled initially
        if (winCamera != null)
        {
            winCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (hasWonOrDestroyed) return;

        ApplyBuoyancy();

        if (Vector3.Distance(previousPosition, transform.position) < 0.01f)
        {
            timeSinceLastMove += Time.deltaTime;
            if (timeSinceLastMove >= maxStuckTime)
            {
                RestartLevel();
            }
        }
        else
        {
            timeSinceLastMove = 0f;
            previousPosition = transform.position;
        }

        if (isResettingRotation)
        {
            float elapsed = Time.time - rotationResetStartTime;
            float t = Mathf.Clamp01(elapsed / rotationResetDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, t);

            if (t >= 1f)
            {
                isResettingRotation = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (hasWonOrDestroyed) return;

        if (!collisionDetected)
        {
            rb.AddForce(currentDirection * waterDrag, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (hasWonOrDestroyed) return;

        if (collision.gameObject.CompareTag("Line"))
        {
            Debug.Log("Collision with line detected.");
            collisionDetected = true;

            lineCollisionCount++;

            rb.AddForce(currentDirection * waterDrag, ForceMode.Acceleration);
        }

        if (collision.gameObject.CompareTag("WinGame"))
        {
            HandleWin();
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleObstacleCollision();
        }

        if (collision.gameObject.CompareTag("EndZone"))
        {
            HandleEndZoneCollision();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Line"))
        {
            rb.AddForce(currentDirection * waterDrag, ForceMode.Acceleration);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Line"))
        {
            lineCollisionCount--;
            if (lineCollisionCount <= 0)
            {
                collisionDetected = false;
                if (!isResettingRotation)
                {
                    isResettingRotation = true;
                    rotationResetStartTime = Time.time;
                }
            }
        }
    }

    private void ApplyBuoyancy()
    {
        if (hasWonOrDestroyed) return;

        Vector3 pos = transform.position;
        float depth = waterLevel + floatHeight - pos.y;

        if (depth > 0)
        {
            float forceFactor = Mathf.Clamp01(depth / floatHeight);
            Vector3 uplift = -Physics.gravity * forceFactor * buoyancyStrength;
            rb.AddForceAtPosition(uplift, transform.position, ForceMode.Acceleration);

            if (pos.y < waterLevel)
            {
                pos.y = waterLevel;
                transform.position = pos;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }
        else
        {
            if (pos.y < waterLevel)
            {
                Vector3 uplift = new Vector3(0, buoyancyStrength, 0);
                rb.AddForce(uplift, ForceMode.Acceleration);
                pos.y = Mathf.Max(pos.y, waterLevel);
                transform.position = pos;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }

        if (rb.velocity.y > 0)
        {
            rb.AddForce(new Vector3(0, -rb.velocity.y * 0.5f, 0), ForceMode.VelocityChange);
        }
    }

    void NextLevel()
    {
        Debug.Log("Next Level button clicked.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void MainMenu()
    {
        Debug.Log("Main Menu button clicked.");
        SceneManager.LoadScene("Main Menu");
    }

    void RestartLevel()
    {
        Debug.Log("Restart Level button clicked.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetCurrentDirection(Vector3 direction)
    {
        currentDirection = direction.normalized;
    }

    public void HandleWin()
    {
        playerAnim.SetBool("Win", true);  // Trigger the Win animation
        Debug.Log("In Trigger");

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        Debug.Log("You Win");
        hasWonOrDestroyed = true;

        StartCoroutine(ShowWinPopupAfterAnimation());

        if (winAudioClip != null && winAudioSource != null)
        {
            winAudioSource.PlayOneShot(winAudioClip);
        }

        // Change to the win camera
        if (winCamera != null && mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false); // Deactivate the main camera
            winCamera.gameObject.SetActive(true);   // Activate the win camera

            // Revert back to the main camera after 3 seconds
            StartCoroutine(RevertToMainCameraAfterDelay(3.0f));
        }
    }

    IEnumerator RevertToMainCameraAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (mainCamera != null && winCamera != null)
        {
            winCamera.gameObject.SetActive(false);  // Deactivate the win camera
            mainCamera.gameObject.SetActive(true);  // Activate the main camera
        }
    }

    IEnumerator ShowWinPopupAfterAnimation()
    {
        Debug.Log("Wait for the win animation to complete");

        yield return new WaitForSeconds(3.0f); // Adjust this time to match your animation length

        if (winPopupPanel != null)
        {
            winPopupPanel.SetActive(true);
            Debug.Log("Win Popup Panel is now active.");
        }
    }

    public void HandleObstacleCollision()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        Debug.Log("Boat is destroyed");
        hasWonOrDestroyed = true;

        if (loseAudioClip != null && loseAudioSource != null)
        {
            loseAudioSource.PlayOneShot(loseAudioClip);
        }

        if (obstacleRetryPopupPanel != null)
        {
            obstacleRetryPopupPanel.SetActive(true);
        }
    }

    public void HandleEndZoneCollision()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        Debug.Log("Boat reached the end zone");
        hasWonOrDestroyed = true;

        if (endZoneAudioClip != null && endZoneAudioSource != null)
        {
            endZoneAudioSource.PlayOneShot(endZoneAudioClip);
        }

        if (endZonePopupPanel != null)
        {
            endZonePopupPanel.SetActive(true);
        }
    }

    private void InitializeUI(GameObject panel, Button button, UnityEngine.Events.UnityAction action)
    {
        if (panel != null && button != null)
        {
            panel.SetActive(false);
            button.onClick.AddListener(action);
        }
    }
}
