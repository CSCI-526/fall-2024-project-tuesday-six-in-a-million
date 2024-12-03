using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SecondLevelSpawnerController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public float enemyInterval = 0.5f;
    public int initialEnemiesPerWave = 3;
    public int enemiesPerWaveIncrement = 1;

    public GameObject Base1;
    public GameObject Base2;

    public int currentWave = 0;
    public int maxWave = 1;
    public float[] chargeTimesPerWave;
    public int totalEnemiesGenerated = 0;
    public int totalEnemiesKilled = 0;

    public GameObject ResetButton;
    public GameObject LevelSelectButton;    // Reference to Level Select Button
    public Text WaveInfo;
    public FlashlightPowerUpdater flashlightPowerUpdater;
    public FlashlightCollider flashlightCollider;
    public List<TowerController> allTowers = new List<TowerController>();

    private bool isSpawning = false;

    private void Awake()
    {
        chargeTimesPerWave = new float[maxWave];
    }

    private void Start()
    {
        TowerSpawner.OnTowerSpawned += AddTowerToList; // Subscribe to tower generation events
        StartCoroutine(LevelSequence());
    }

    private void OnDestroy()
    {
        TowerSpawner.OnTowerSpawned -= AddTowerToList; // Unsubscribe
    }

    private void AddTowerToList(TowerController tower)
    {
        allTowers.Add(tower);
        Debug.Log("Added tower, total tower count: " + allTowers.Count);
    }

    public void AddChargeTime(float deltaTime)
    {
        if (currentWave > 0 && currentWave <= maxWave)
        {
            chargeTimesPerWave[currentWave - 1] += deltaTime;
            Debug.Log($"Current wave: {currentWave}, total charging time: {chargeTimesPerWave[currentWave - 1]:F2} seconds");
        }
        else
        {
            Debug.LogWarning("Cannot update charging time");
        }
    }

    private IEnumerator LevelSequence()
    {
        Debug.Log("Level started");

        while (currentWave < maxWave)
        {
            currentWave++;
            WaveInfo.text = "Wave: " + currentWave + " / " + maxWave;

            Debug.Log($"Starting wave {currentWave}");

            if (currentWave == 1)
            {
                yield return StartCoroutine(SpawnWave(spawnPoint1, initialEnemiesPerWave, 1));
            }
            else if (currentWave == 2)
            {
                yield return StartCoroutine(SpawnWave(spawnPoint2, initialEnemiesPerWave, 2));
            }
            else if (currentWave == 3)
            {
                StartCoroutine(SpawnWave(spawnPoint1, initialEnemiesPerWave + 1, 1));
                yield return new WaitForSeconds(5);
                yield return StartCoroutine(SpawnWave(spawnPoint2, initialEnemiesPerWave + 1, 2));
            }
            else if (currentWave == 4)
            {
                StartCoroutine(SpawnWave(spawnPoint1, initialEnemiesPerWave + 2, 1));
                yield return new WaitForSeconds(5);
                yield return StartCoroutine(SpawnWave(spawnPoint2, initialEnemiesPerWave + 2, 2));
            }
            else if (currentWave == 5)
            {
                StartCoroutine(SpawnWave(spawnPoint1, initialEnemiesPerWave + 3, 1));
                yield return new WaitForSeconds(5);
                yield return StartCoroutine(SpawnWave(spawnPoint2, initialEnemiesPerWave + 3, 2));
            }

            // Wait until all enemies in the wave are defeated
            while (isSpawning || GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return null;
            }

            Debug.Log($"Wave {currentWave} spawning complete");
        }

        EndLevel();
    }

    private IEnumerator SpawnWave(Transform spawnPoint, int enemiesInWave, int health = 2)
    {
        isSpawning = true;

        Debug.Log($"Wave {currentWave}: Spawning {enemiesInWave} enemies at {spawnPoint.name}");

        for (int i = 0; i < enemiesInWave; i++)
        {
            SpawnEnemy(spawnPoint, health);
            yield return new WaitForSeconds(enemyInterval);
        }

        isSpawning = false;
    }

    private void SpawnEnemy(Transform spawnPoint, int health = 2)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemy.tag = "Enemy";
        enemy.SetActive(true); // Ensure the enemy is active

        Debug.Log($"Spawned enemy at position: {spawnPoint.position} with active state: {enemy.activeSelf}");

        EnemyController2 enemyController = enemy.GetComponent<EnemyController2>();
        if (enemyController != null)
        {
            enemyController.spawnerController = this;
            enemyController.health = health;
            enemyController.Base1 = Base1;
            enemyController.Base2 = Base2;
            enemyController.flashlightCollider = flashlightCollider;
            enemyController.flashlightPowerUpdater = flashlightPowerUpdater;
            enemyController.ResetButton = ResetButton;

            // Set enemy color based on health
            SetEnemyColorBasedOnHealth(enemyController);
        }
        else
        {
            Debug.LogError("EnemyController2 component missing on enemyPrefab.");
        }

        totalEnemiesGenerated++;
    }

    private void SetEnemyColorBasedOnHealth(EnemyController2 enemyController)
    {
        Renderer enemyRenderer = enemyController.GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            if (enemyController.health == 1)
            {
                enemyRenderer.material.color = Color.red; // Red for health = 1
            }
            else if (enemyController.health == 2)
            {
                enemyRenderer.material.color = Color.yellow; // Yellow for health = 2
            }
            else if (enemyController.health == 3)
            {
                enemyRenderer.material.color = Color.green; // Green for health = 3
            }
        }
        else
        {
            Debug.LogError("Renderer not found on EnemyController2.");
        }
    }

    private void EndLevel()
    {
        Debug.Log("Level complete");

        CollectAnalyticsData();
    }

    private void CollectAnalyticsData()
    {
        // Pause the game on victory
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Display victory text
        GameObject.Find("Win").GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
        ResetButton.SetActive(true);
        LevelSelectButton.SetActive(true);

        //
        string sceneName = SceneManager.GetActiveScene().name;
        int levelNumber = int.Parse(sceneName.Replace("Level ", ""));
        Debug.Log("Level number: " + levelNumber);

        // Collect tower data
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

        // Collect flashlight usage durations
        List<float> flashlightDurations = flashlightPowerUpdater.GetUsageDurations();

        // Record game data
        if (FirebaseDataSender.Instance != null)
        {
            FirebaseDataSender.Instance.SendGameResult(levelNumber, true, currentWave, Time.timeSinceLevelLoad, flashlightDurations, towerDataList, chargeTimesPerWave);
            Debug.Log("SendGameResult called successfully.");
        }
        else
        {
            Debug.LogError("FirebaseDataSender Instance is null.");
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.GameOver();
            }

        // Prevent multiple triggers
        this.enabled = false;
    }
}
