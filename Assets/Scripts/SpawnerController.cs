using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerController : MonoBehaviour
{
    public GameObject enemyPrefab;          // Enemy prefab to spawn
    public Transform spawnPoint;            // Spawn point for enemies
    public float enemyInterval = 1.0f;      // Interval between spawning enemies

    public int currentWave = 0;             // Current wave number
    public int maxWave = 10;                // Maximum number of waves
    public int initialEnemiesPerWave = 5;   // Initial number of enemies per wave
    public int enemiesPerWaveIncrement = 2; // Increment of enemies per wave

    private int enemiesSpawnedInWave = 0;   // Number of enemies spawned in the current wave
    private bool isSpawning = false;        // Whether the wave is currently spawning
    private bool tutorialComplete = false;  // Track if the tutorial is complete

    public int totalEnemiesGenerated = 0;   // Total enemies generated
    public int totalEnemiesKilled = 0;      // Total enemies killed

    public GameObject ResetButton;          // Reference to Reset Button
    public Text WaveInfo;                   // Text displaying wave information
                    // Text displaying welcome message

    public FirebaseDataSender firebaseDataSender;  // Reference to FirebaseDataSender
    public FlashlightPowerUpdater flashlightPowerUpdater;

    public List<TowerController> allTowers = new List<TowerController>(); // List of all towers

    private void Start()
    {
        TowerSpawner.OnTowerSpawned += AddTowerToList; // Subscribe to tower generation events
         // Display welcome message at start
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        TowerSpawner.OnTowerSpawned -= AddTowerToList;
    }

    // Method to spawn a single enemy for the tutorial phase
    public void SpawnSingleEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position + new Vector3(0, -1, 0), spawnPoint.rotation);
        enemy.SetActive(true);
        enemy.tag = "Enemy"; 

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.spawnerController = this;
        }
    }

    // Coroutine to spawn a wave of enemies
    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        enemiesSpawnedInWave = 0;
        int enemiesInThisWave = initialEnemiesPerWave + (currentWave - 1) * enemiesPerWaveIncrement;

        for (int i = 0; i < enemiesInThisWave; i++)
        {
            SpawnEnemy();
            enemiesSpawnedInWave++;      // Increase count of spawned enemies in the wave
            totalEnemiesGenerated++;     // Increment total enemies generated
            yield return new WaitForSeconds(enemyInterval);
        }
        isSpawning = false;
    }
    
    // Add tower to list when it is spawned
    private void AddTowerToList(TowerController tower)
    {
        allTowers.Add(tower);
        Debug.Log("Added tower, total tower count: " + allTowers.Count);
    }

    // Spawn an enemy
    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position + new Vector3(0, -1, 0), spawnPoint.rotation);
        enemy.SetActive(true);
        enemy.tag = "Enemy"; 

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.spawnerController = this;
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0) return; // Skip if the game is paused

        WaveInfo.text = "Wave: " + currentWave + " / " + maxWave;

        // Delay 3 seconds before starting the first wave
        if (Time.timeSinceLevelLoad < 3)
        {
            Debug.Log("Delay 3 seconds before starting the first wave");
            return;
        }
        
        // Hide the welcome text after the initial delay
        

        // Only start spawning waves after the tutorial is complete
        if (!tutorialComplete) return;

        // Check if need to start next wave
        if (!isSpawning && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            if (currentWave < maxWave)
            {
                currentWave++;
                StartCoroutine(SpawnWave());
            }
        }

        // Check victory condition
        if (!isSpawning && currentWave == maxWave && totalEnemiesKilled == totalEnemiesGenerated)
        {
            // Game win
            Time.timeScale = 0;

            // Show the win text
            GameObject.Find("Win").GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);

            // Make the reset button visible
            ResetButton.SetActive(true);

            // Collect data
            List<TowerData> towerDataList = new List<TowerData>();
            foreach (TowerController tower in allTowers)
            {
                TowerData data = new TowerData
                {
                    totalChargeTime = tower.totalChargeTime,
                    totalKillCount = tower.totalKillCount
                };
                towerDataList.Add(data);
            }

            List<float> flashlightDurations = flashlightPowerUpdater.GetUsageDurations();

            // Record game data
            FirebaseDataSender.Instance.SendGameResult(true, currentWave, Time.timeSinceLevelLoad, flashlightDurations, towerDataList);

            // Prevent multiple triggers
            this.enabled = false;
        }
    }

    // Method to complete the tutorial and allow waves to start spawning
    public void CompleteTutorial()
    {
        tutorialComplete = true;
    }
}
