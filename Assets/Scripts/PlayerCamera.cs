using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject player;   // The player object the camera follows
    public float distance = 7f; // Distance from the player
    public float height = 5f;   // Height above the player
    public float sensitivity = 100f; // Mouse sensitivity for rotation
    public float verticalOffset = 2f; // Vertical offset for the camera

    private Vector3 offset;     // Offset from the player

    void Start()
    {
        // Initial offset based on given height and distance
        offset = new Vector3(0, height, -distance);
        LockCursor();
    }

    void LateUpdate()
    {
        // Rotate the camera around the player based on mouse input
        float horizontalInput = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // Update the offset by rotating it around the Y-axis of the player
        offset = Quaternion.AngleAxis(horizontalInput, Vector3.up) * offset;

        if (player == null)
        {
            UnlockCursor();
            return;
        }

        // Set the camera's position to follow the player with the updated offset
        transform.position = player.transform.position + offset;

        Vector3 LookAt = player.transform.position + Vector3.up * verticalOffset;

        // Always make the camera look at the player
        transform.LookAt(LookAt);
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
