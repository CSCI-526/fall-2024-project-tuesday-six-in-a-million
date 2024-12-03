using System.Collections;
using UnityEngine;

public class ArrowGuide : MonoBehaviour
{
    public Transform player; // The player's Transform
    public Transform[] targets; // List of targets (charging stations, placement zones, etc.)
    private Transform currentTarget; // Current target the arrow should point to

    private void Update()
    {
        if (targets.Length == 0 || player == null) return;

        // Find the nearest target
        float minDistance = float.MaxValue;
        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(player.position, target.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                currentTarget = target;
            }
        }

        if (currentTarget == null) return;

        // Update arrow's position and rotation
        Vector3 direction = currentTarget.position - player.position;
        direction.y = 0; // Keep the arrow flat on the horizontal plane
        transform.position = player.position + Vector3.up * 1.5f; // Position above the player
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void ShowArrow(Transform[] newTargets)
    {
        targets = newTargets;
        gameObject.SetActive(true); // Enable the arrow
    }

    public void HideArrow()
    {
        gameObject.SetActive(false); // Hide the arrow
    }
}
