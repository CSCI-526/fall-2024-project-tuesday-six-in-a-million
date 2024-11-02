using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerController : MonoBehaviour
{
    public GameObject enemyPrefab;      //
    public Transform spawnPoint;        // spawn point
    public float enemyInterval = 1.0f;  // enemy interval

    public int currentWave = 0;         // current wave 
    public int maxWave = 10;            // max wave
    public int enemiesPerWave = 5;      // enemies per wave

    private int enemiesSpawnedInWave = 0;   // enemies spawned in th current wave
    private bool isSpawning = false;        // whether is spawning

    public int totalEnemiesGenerated = 0;   // total enemies generated
    public int totalEnemiesKilled = 0;      // total enemies killed

    public GameObject ResetButton;  // ResetButton
    public Text WaveInfo;           // WaveInfo
    public Text WelcomeText;        // WelcomeText


    public FirebaseDataSender firebaseDataSender;  // Reference to FirebaseDataSender
    public FlashlightPowerUpdater flashlightPowerUpdater;

    public List<TowerController> allTowers = new List<TowerController>();   

    void Start()
    {
         TowerSpawner.OnTowerSpawned += AddTowerToList; // Subscribe to tower generation events
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        TowerSpawner.OnTowerSpawned -= AddTowerToList;
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        enemiesSpawnedInWave = 0;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            enemiesSpawnedInWave++;      // enemies spawned in th current wave
            totalEnemiesGenerated++;  // total enemies generated plus one
            yield return new WaitForSeconds(enemyInterval);
        }
        isSpawning = false;
    }
    
    void AddTowerToList(TowerController tower)
    {
        allTowers.Add(tower);
        Debug.Log("added tower, total tower count：" + allTowers.Count);
    }

    void SpawnEnemy()
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

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        WaveInfo.text = "Wave: " + currentWave + " / " + maxWave;

        // delay 3 seconds before starting the first wave
        if (Time.timeSinceLevelLoad < 3)
        {
            Debug.Log("Delay 3 seconds before starting the first wave");
            return;
        }
        WelcomeText.gameObject.SetActive(false);

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

            //collect data
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
            FirebaseDataSender.Instance.SendGameResult(true, currentWave, Time.timeSinceLevelLoad, flashlightDurations,towerDataList);

            // Prevent multiple triggers
            this.enabled = false;
        }
    }
}
