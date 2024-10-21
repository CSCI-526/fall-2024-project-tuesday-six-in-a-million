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

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemy.GetComponent<EnemyController>().isPrefab = false;
        enemy.tag = "Enemy"; 
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
        if (!isSpawning && GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
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

            // Record game data
            FirebaseDataSender.Instance.SendGameResult(true, currentWave, Time.timeSinceLevelLoad);

            // Prevent multiple triggers
            this.enabled = false;
        }
    }
}
