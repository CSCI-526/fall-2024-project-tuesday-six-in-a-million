using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public GameObject enemyPrefab;      // 敌人预制体
    public Transform spawnPoint;        // 敌人生成点
    public float enemyInterval = 1.0f;  // 敌人生成间隔

    public int currentWave = 1;         // 当前波数
    public int maxWave = 10;            // 最大波数
    public int enemiesPerWave = 5;      // 每波敌人数量

    private int enemiesSpawnedInWave = 0;   // 当前波已生成的敌人数
    private bool isSpawning = false;        // 是否正在生成敌人

    public int totalEnemiesGenerated = 0;   // 生成的敌人总数
    public int totalEnemiesKilled = 0;      // 被消灭的敌人总数

    public GameObject ResetButton;  // 重置按钮
    public GameDataRecorder dataRecorder;   // 数据记录器

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        enemiesSpawnedInWave = 0;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            enemiesSpawnedInWave++;
            totalEnemiesGenerated++;  // 总生成敌人数量增加
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

        // 检查是否需要开始下一波
        if (!isSpawning && GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
        {
            if (currentWave < maxWave)
            {
                currentWave++;
                StartCoroutine(SpawnWave());
            }

        }

        // 检查胜利条件
        if (!isSpawning  && currentWave == maxWave && totalEnemiesKilled == totalEnemiesGenerated)
        {
            // 游戏胜利
            Time.timeScale = 0;

            // make the game over text text alpha 1
            GameObject.Find("Win").GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
    


            // make the reset button visible
            ResetButton.SetActive(true);

            // 防止重复触发
            this.enabled = false;
        }
    }
}