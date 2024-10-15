using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 20.0f;
    private float jumpForce = 150.0f;
    private bool isGrounded = true;
    private float horizontalInput;
    private float forwardInput;
    private Rigidbody rb;

    public GameObject towerPrefab;
    public GameObject helpText;
    public LayerMask towerLayer;
    public float mouseSensitivity = 100.0f;  // Sensitivity for mouse movement
    private float mouseX;

    public FlashlightPowerUpdater flashlight;  // Reference to the UI updater script

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void ToggleHelp() {
        print("Toggling help text");
        if (helpText.activeSelf) {
            print("Deactivating help text");
            helpText.SetActive(false);
            Time.timeScale = 1;
        } else {
            helpText.SetActive(true);
            Time.timeScale = 0;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHelp();
        }
        if (Time.timeScale == 0)
        {
            return;  // Prevent movement and interactions if game is paused
        }

        // Toggle help text with h key

        // Handle flashlight rotation
        MouseControl();

        // Input for movement
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // Toggle flashlight with F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.toggleFlashlight();
        }

        // Move player
        MovePlayer();

        // Create a tower when LeftShift is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CreateTower();
        }

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

    void CreateTower()
    {
        GameObject tower = Instantiate(towerPrefab, transform.position + new Vector3(2, 0, 0), transform.rotation);
        Debug.Log("Tower spawned successfully.");
    }
}
