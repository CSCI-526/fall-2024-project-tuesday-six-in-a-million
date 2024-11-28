using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float speed = 20.0f;
    private float jumpForce = 150.0f;
    private bool isGrounded = true;

    private bool isGameOver = false;    // Track if the game is over
    private float horizontalInput;
    private float forwardInput;
    private Rigidbody rb;
    public float spotlightYOffset = 7f;
    public float spotlightZOffset = -3f;

    public GameObject towerPrefab;
    public GameObject helpText;
    public float mouseSensitivity = 100.0f;  // Sensitivity for mouse movement
    public TowerSpawner towerSpawner;  // Reference to the tower spawner script
    public Text alertText;

    public FlashlightPowerUpdater flashlight;  // Reference to the UI updater script
    public GameObject spotlight;  // Reference to the spotlight object
    public GameObject ResetButton;  // Reference to the reset button object

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void ToggleHelp() {
        print("Toggling help text");
        if (helpText.activeSelf) {
            print("Deactivating help text");
            if (!ResetButton.activeSelf) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            helpText.SetActive(false);
            if (!isGameOver)
            {
                Time.timeScale = 1;
            }
        } else {
            helpText.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHelp();
        }
        if (Time.timeScale == 0 || isGameOver)
            {
                return;
            }
        // Toggle help text with h key

        // Handle flashlight rotation
        MouseControl();

        // Input for movement
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // Toggle flashlight with mouse click
        if (Input.GetMouseButtonDown(0))
        {
            flashlight.toggleFlashlight();
        }

        // Move player
        MovePlayer();

        // Create a tower when LeftShift is pressed
        towerSpawner.DetectSpawnTower();

        // Jump when Space is pressed
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void MouseControl()
    {
        Quaternion cameraRotation = Camera.main.transform.rotation;

        // Decompose the rotation into Euler angles (degrees for each axis)
        Vector3 cameraEulerAngles = cameraRotation.eulerAngles;

        // Set flashlight rotation, but keep the X (pitch) axis fixed so it doesn't point down
        flashlight.transform.rotation = Quaternion.Euler(0, cameraEulerAngles.y, cameraEulerAngles.z);

        Vector3 spotlightOffset = new Vector3(0, spotlightYOffset, spotlightZOffset);

        // Calculate the new rotation for the spotlight based on the camera's Y rotation
        Quaternion spotlightRotation = Quaternion.Euler(spotlight.transform.rotation.eulerAngles.x, cameraEulerAngles.y, spotlight.transform.rotation.eulerAngles.z);
        
        // Apply the rotation to the spotlight
        spotlight.transform.rotation = spotlightRotation;

        // Recalculate the spotlight's position based on the player's position and offset
        // Rotate the offset based on the camera's Y rotation
        Vector3 rotatedOffset = Quaternion.Euler(0, cameraEulerAngles.y, 0) * spotlightOffset;

        // Set the new position of the spotlight relative to the player's position
        spotlight.transform.position = transform.position + rotatedOffset;
    }

    void MovePlayer()
    {
        // Move the player based on camera direction
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 direction = forward * forwardInput + right * horizontalInput;
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
    }

    void Jump()
    {
        // Apply jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

        public void GameOver()
    {
        isGameOver = true;
    }


}
