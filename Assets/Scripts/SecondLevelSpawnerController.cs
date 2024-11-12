using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondLevelSpawnerController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public float enemyInterval = 1.0f;
    public int initialEnemiesPerWave = 5;
    public int enemiesPerWaveIncrement = 2;

    public GameObject Base1;
    public GameObject Base2;

    public int currentWave = 0;
    public int maxWave = 5;
    public float[] chargeTimesPerWave;
    public int totalEnemiesGenerated = 0;
    public int totalEnemiesKilled = 0;

    public GameObject ResetButton;
    public Text WaveInfo;
    public FirebaseDataSender firebaseDataSender;
    public FlashlightPowerUpdater flashlightPowerUpdater;
    public FlashlightCollider flashlightCollider;
    public List<TowerController> allTowers = new List<TowerController>();

    private bool isSpawning = false;

    private void Start()
    {
        chargeTimesPerWave = new float[maxWave];
        StartCoroutine(LevelSequence());
    }

    private IEnumerator LevelSequence()
    {
        Debug.Log("Level started");

        while (currentWave < maxWave)
        {
            currentWave++;
            WaveInfo.text = "Wave: " + currentWave + " / " + maxWave;

            Debug.Log($"Starting wave {currentWave}");

            // Configure enemy health for each wave
            if (currentWave < 4)
            {
                enemyPrefab.GetComponent<EnemyController2>().health = 1; // Set health to 1 for early waves
            }
            else
            {
                enemyPrefab.GetComponent<EnemyController2>().health = 2; // Set health to 2 for later waves
            }

            if (currentWave == 1)
            {
                yield return StartCoroutine(SpawnWave(spawnPoint1));
            }
            else if (currentWave == 2)
            {
                yield return StartCoroutine(SpawnWave(spawnPoint2));
            }
            else if (currentWave == 3)
            {
                StartCoroutine(SpawnWave(spawnPoint1));
                yield return new WaitForSeconds(5);
                yield return StartCoroutine(SpawnWave(spawnPoint2));
            }
            else if (currentWave >= 4)
            {
                StartCoroutine(SpawnWave(spawnPoint1));
                yield return new WaitForSeconds(5);
                yield return StartCoroutine(SpawnWave(spawnPoint2));
            }

            while (isSpawning || GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return null;
            }

            Debug.Log($"Wave {currentWave} spawning complete");
        }

        EndLevel();
    }

    private IEnumerator SpawnWave(Transform spawnPoint)
    {
        isSpawning = true;
        int enemiesInWave = initialEnemiesPerWave + (currentWave - 1) * enemiesPerWaveIncrement;

        Debug.Log($"Wave {currentWave}: Spawning {enemiesInWave} enemies at {spawnPoint.name}");

        for (int i = 0; i < enemiesInWave; i++)
        {
            SpawnEnemy(spawnPoint);
            yield return new WaitForSeconds(enemyInterval);
        }

        isSpawning = false;
    }

    private void SpawnEnemy(Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemy.tag = "Enemy";
        enemy.SetActive(true);  // Ensure the enemy is active

        Debug.Log($"Spawned enemy at position: {spawnPoint.position} with active state: {enemy.activeSelf}");

        EnemyController2 enemyController = enemy.GetComponent<EnemyController2>();
        if (enemyController != null)
        {
            enemyController.spawnerController = this;
            enemyController.Base1 = Base1;
            enemyController.Base2 = Base2;
            enemyController.flashlightCollider = flashlightCollider;  // Make sure flashlightCollider is assigned

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
        GameObject.Find("Win").GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
        ResetButton.SetActive(true);

        CollectAnalyticsData();
    }

    private void CollectAnalyticsData()
    {
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
        firebaseDataSender.SendGameResult(true, currentWave, Time.timeSinceLevelLoad, flashlightDurations, towerDataList, chargeTimesPerWave);
    }
}
