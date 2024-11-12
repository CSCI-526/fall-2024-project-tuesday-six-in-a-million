using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : MonoBehaviour
{
    public int health = 1;
    public int reward = 50;
    public int moveSpeed = 5;
    public GameObject Base1;
    public GameObject Base2;
    public GameObject ResetButton;

    public Light flashlight;
    public FlashlightCollider flashlightCollider;
    public FlashlightPowerUpdater flashlightPowerUpdater;
    public FirebaseDataSender firebaseDataSender;
    public SecondLevelSpawnerController spawnerController;

    private Renderer enemyRenderer;
    public Color highlightColor = Color.magenta;

    private void Start()
    {
        Debug.Log("EnemyController2 Start() called"); // Debug: Enemy initialized
        enemyRenderer = GetComponent<Renderer>();
        HighlightEnemy();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (Base1 == null || Base2 == null)
        {
            Debug.LogError("Base1 or Base2 is not assigned in EnemyController2."); // Debug: Missing base reference
            return;
        }

        GameObject targetBase = Vector3.Distance(transform.position, Base1.transform.position) < Vector3.Distance(transform.position, Base2.transform.position)
            ? Base1
            : Base2;

        int adjustedSpeed = flashlightCollider != null && flashlightCollider.IsHitByFlashlight(gameObject) ? moveSpeed / 2 : moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, targetBase.transform.position, adjustedSpeed * Time.deltaTime);
    }

    private void HighlightEnemy()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = highlightColor;
            Debug.Log("Enemy highlighted with color: " + highlightColor); // Debug: Enemy highlight color applied
        }
        else
        {
            Debug.LogError("Renderer not found on enemy."); // Debug: Missing renderer
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with bases
        if (collision.gameObject == Base1 || collision.gameObject == Base2)
        {
            Debug.Log($"Enemy collided with {collision.gameObject.name} and will be destroyed."); // Debug: Collision detected
            Destroy(gameObject);
            if (spawnerController != null)
            {
                spawnerController.totalEnemiesKilled++;
                Debug.Log($"Total enemies killed: {spawnerController.totalEnemiesKilled}"); // Debug: Enemy killed count updated
            }
            else
            {
                Debug.LogError("SpawnerController reference is missing in EnemyController2."); // Debug: Missing spawner reference
            }
        }
        // Handle collision with player
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with enemy and will be destroyed."); // Debug: Player death detected

            // Trigger player death actions
            Destroy(collision.gameObject);  // Destroy player GameObject

            // Show Game Over UI and reset options
            if (ResetButton != null)
            {
                ResetButton.SetActive(true);
            }

            GameObject gameOverText = GameObject.Find("GameOver");
            if (gameOverText != null)
            {
                gameOverText.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
            }

            // Stop time to show game over effect
            Time.timeScale = 0;
        }
    }
}
