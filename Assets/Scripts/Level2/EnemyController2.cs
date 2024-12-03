using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController2 : MonoBehaviour
{
    public int health = 2; // Updated default health to 2 (two hits to kill)
    public int reward = 50;
    public int moveSpeed = 5;
    public GameObject Base1;
    public GameObject Base2;
    public GameObject ResetButton;
    public GameObject LevelSelectButton;
  

    public Light flashlight;
    public FlashlightCollider flashlightCollider;
    public FlashlightPowerUpdater flashlightPowerUpdater;
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

        UpdateEnemyColor();
        // Ensure Base1 and Base2 are assigned
        if (Base1 == null || Base2 == null)
        {
            Debug.LogError("Base1 or Base2 is not assigned in EnemyController2."); // Debug: Missing base reference
            return;
        }

        // Select the closer base as the target
        GameObject targetBase = Vector3.Distance(transform.position, Base1.transform.position) <
                                Vector3.Distance(transform.position, Base2.transform.position)
            ? Base1
            : Base2;

        // Adjust speed if hit by flashlight
        int adjustedSpeed = flashlightCollider != null && flashlightCollider.IsHitByFlashlight(gameObject)
            ? moveSpeed / 2 // Halve the speed when under flashlight
            : moveSpeed;

        // Move towards the target base
        transform.position = Vector3.MoveTowards(transform.position, targetBase.transform.position,
            adjustedSpeed * Time.deltaTime);
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
        if (collision.gameObject == Base1 || collision.gameObject == Base2 || collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Enemy collided with {collision.gameObject.name}. Game Over."); // Debug: Collision detected

            // Collect analytics and send to Firebase
            CollectAnalyticsData(false);

            // Stop time to show game over effect
            Time.timeScale = 0;

            // Destroy the enemy
            Destroy(gameObject);
        }
        // Handle collision with bullets
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Enemy hit by bullet."); // Debug: Bullet hit detected

            // Reduce health
            health--;

            // Check if the enemy is dead
            if (health <= 0)
            {
                Debug.Log("Enemy killed."); // Debug: Enemy death
                spawnerController.totalEnemiesKilled++;

                // Destroy the enemy
                Destroy(gameObject);
            }

            // Destroy the bullet
            Destroy(collision.gameObject);
        }
    }



    private void UpdateEnemyColor()
    {
        if (enemyRenderer != null)
        {
            if (health == 3) {
                enemyRenderer.material.color = Color.green; // Change to green when full health
            }
            else if (health == 2)
            {
                enemyRenderer.material.color = Color.yellow; // Change to highlight color when full health
            }
            
            else if (health == 1)
            {
                enemyRenderer.material.color = Color.red; // Change to red when one hit left
            }
            else if (health <= 0)
            {
                enemyRenderer.material.color = Color.black; // Optional: Set to black when dead
            }
        }
    }

    private void CollectAnalyticsData(bool isWin)
    {
        Debug.Log("CollectAnalyticsData called with isWin = " + isWin);

        // Display game over UI
        if (ResetButton != null)
        {
            ResetButton.SetActive(true);
        }
        LevelSelectButton.SetActive(true);
        //unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Get level number
        string sceneName = SceneManager.GetActiveScene().name;
        int levelNumber = int.Parse(sceneName.Replace("Level ", ""));
        Debug.Log("Level number: " + levelNumber);

        GameObject gameOverText = GameObject.Find(isWin ? "Win" : "GameOver");
        if (gameOverText != null)
        {
            gameOverText.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
        }

        // Collect tower data
        List<TowerData> towerDataList = new List<TowerData>();
        if (spawnerController != null)
        {
            List<TowerController> allTowers = spawnerController.allTowers;
            Debug.Log("Number of Towers: " + allTowers.Count);
            foreach (TowerController tower in allTowers)
            {
                TowerData data = new TowerData
                {
                    totalChargeTime = tower.totalChargeTime,
                    totalKillCount = tower.totalKillCount
                };
                towerDataList.Add(data);
                Debug.Log("Tower Charge Time: " + data.totalChargeTime + ", Kill Count: " + data.totalKillCount);
            }
        }
        else
        {
            Debug.LogError("SpawnerController reference is missing in EnemyController2.");
        }

        // Collect flashlight usage durations
        if (flashlightPowerUpdater != null)
        {
            flashlightPowerUpdater.AddDuration();
            List<float> flashlightDurations = flashlightPowerUpdater.GetUsageDurations();

            // Get current wave and charging times
            int currentWave = spawnerController != null ? spawnerController.currentWave : 0;
            float[] chargeTimesPerWave = spawnerController != null ? spawnerController.chargeTimesPerWave : new float[0];

            // Record game data
            if (FirebaseDataSender.Instance != null)
            {
                FirebaseDataSender.Instance.SendGameResult(levelNumber, isWin, currentWave, Time.timeSinceLevelLoad, flashlightDurations,
                    towerDataList, chargeTimesPerWave);
                Debug.Log("SendGameResult called successfully.");
            }
            else
            {
                Debug.LogError("FirebaseDataSender Instance is null.");
            }
        }
        else
        {
            Debug.LogError("FlashlightPowerUpdater reference is missing in EnemyController2.");
        }
        
        // Game over
        PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.GameOver();
            }

            this.enabled = false;
        // Prevent multiple triggers
        this.enabled = false;
    }
}
