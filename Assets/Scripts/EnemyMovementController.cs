using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public GameObject path;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    // public int moveSpeed = 5;
    private LineRenderer lineRenderer;
    
    private EnemyController2 enemyController; // prive reference to the EnemyController2 script

    // Start is called before the first frame update
    void Start()
    {

        enemyController = GetComponent<EnemyController2>(); // Get the EnemyController2 script attached to the same GameObject

        waypoints = path.GetComponentsInChildren<Transform>();
        Debug.Log(waypoints.Length);

        lineRenderer = path.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = path.gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = waypoints.Length; // Set number of points
        lineRenderer.startWidth = 0.1f; // Width of the line
        lineRenderer.endWidth = 0.1f; 
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Basic material
        lineRenderer.startColor = Color.red; // Line color
        lineRenderer.endColor = Color.red;

        // generate a visual representation of the path
        for (int i = 0; i < waypoints.Length; i++)
        {
            // Debug.DrawLine(waypoints[i].position, waypoints[i + 1].position, Color.red, 10000f);
            // draw a line between the waypoints
            Vector3 adjustedPosition = new Vector3(
                waypoints[i].position.x,
                waypoints[i].position.y - 2.5f,
                waypoints[i].position.z
            );

            // Set the adjusted position in the LineRenderer
            lineRenderer.SetPosition(i, adjustedPosition);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        Move();
    }

    void Move()
    {
         if (currentWaypointIndex < waypoints.Length && enemyController != null)
    {
        // default speed
        float adjustedSpeed = enemyController.moveSpeed;

        // if the enemy is hit by the flashlight, halve the speed
        if (enemyController.flashlightCollider != null && enemyController.flashlightCollider.IsHitByFlashlight(gameObject))
        {
            adjustedSpeed /= 2; // speed slows down by half
        }

        // update the position of the enemy
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, adjustedSpeed * Time.deltaTime);
        if (transform.position == waypoints[currentWaypointIndex].position)
        {
            currentWaypointIndex++;
        }
    }
    }
}
