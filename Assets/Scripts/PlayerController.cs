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
    public Light flashlight;
    public float flashlightPower = 100f;  // Flashlight's maximum power
    public float powerDrainAmount = 5f;   // Amount of power drained per tower charge
    public float powerRechargeRate = 5f;  // How fast flashlight recharges when off
    public float flashlightRange = 20f;   // Range of flashlight to interact with towers
    public LayerMask towerLayer;
    public float mouseSensitivity = 100.0f;  // Sensitivity for mouse movement
    private float mouseX;

    public FlashlightPowerUpdater uiUpdater;  // Reference to the UI updater script

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        flashlight.enabled = false;  // Start with the flashlight off
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;  // Prevent movement and interactions if game is paused
        }

        // Handle flashlight rotation
        MouseControl();

        // Input for movement
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // Toggle flashlight with F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlightPower > 0)
            {
                flashlight.enabled = !flashlight.enabled;  // Toggle flashlight on/off
            }
        }

        // Handle flashlight interactions if it's on
        if (flashlight.enabled)
        {
            HandleFlashlightCollision();
        }
        else if (flashlightPower < 100)
        {
            // Recharge flashlight when it's off
            flashlightPower += powerRechargeRate * Time.deltaTime;
            UpdateFlashlightUI();
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
        // Capture mouse movement for rotating the flashlight (only on Y-axis)
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;

        // Rotate the flashlight only around the Y-axis
        flashlight.transform.localRotation = Quaternion.Euler(0, mouseX, 0);
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
        if (flashlightPower > 0)
        {
            // Spawn a tower to the right of the player
            GameObject tower = Instantiate(towerPrefab, transform.position + new Vector3(2, 0, 0), transform.rotation);
            Debug.Log("Tower spawned successfully.");
        }
        else
        {
            Debug.Log("Not enough flashlight power to place a tower.");
        }
    }

    void HandleFlashlightCollision()
    {
        Ray ray = new Ray(flashlight.transform.position, flashlight.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, flashlightRange, towerLayer))
        {
            TowerController tower = hit.collider.GetComponent<TowerController>();
            if (tower != null && !tower.isCharged)
            {
                // Charge the tower
                tower.ChargeTower();

                // Drain flashlight power by 5 when charging a tower
                flashlightPower -= powerDrainAmount;
                flashlightPower = Mathf.Clamp(flashlightPower, 0, 100);  // Prevent negative power

                // Update the UI after draining power
                UpdateFlashlightUI();

                Debug.Log("Tower activated, flashlight power drained by 5.");

                if (flashlightPower <= 0)
                {
                    flashlightPower = 0;
                    flashlight.enabled = false;  // Turn off flashlight if power is depleted
                }
            }
        }
    }

    // Function to update the flashlight power on the UI
    void UpdateFlashlightUI()
    {
        if (uiUpdater != null)
        {
            uiUpdater.UpdateFlashlightUI(flashlightPower);
        }
    }
}
