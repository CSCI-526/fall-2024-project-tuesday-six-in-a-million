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
            Debug.Log($"Enemy collided with {collision.gameObject.name}. Game Over."); // Debug: Collision detected

            // 收集数据并发送到 Firebase
            CollectAnalyticsData(false);

            // Stop time to show game over effect
            Time.timeScale = 0;

            // Destroy enemy
            Destroy(gameObject);
        }
        // Handle collision with player
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with enemy. Game Over."); // Debug: Player death detected

            // Destroy player GameObject
            Destroy(collision.gameObject);

            // 收集数据并发送到 Firebase
            CollectAnalyticsData(false);

            // Stop time to show game over effect
            Time.timeScale = 0;

            // Destroy enemy
            Destroy(gameObject);
        }
    }

    private void CollectAnalyticsData(bool isWin)
    {
        Debug.Log("CollectAnalyticsData called with isWin = " + isWin);

        // 显示游戏结束 UI
        if (ResetButton != null)
        {
            ResetButton.SetActive(true);
        }

        GameObject gameOverText = GameObject.Find(isWin ? "Win" : "GameOver");
        if (gameOverText != null)
        {
            gameOverText.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
        }

        // 收集塔的数据
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

        // 收集手电筒使用时间
        if (flashlightPowerUpdater != null)
        {
            flashlightPowerUpdater.AddDuration();
            List<float> flashlightDurations = flashlightPowerUpdater.GetUsageDurations();

            // 获取当前波次和充电时间
            int currentWave = spawnerController != null ? spawnerController.currentWave : 0;
            float[] chargeTimesPerWave = spawnerController != null ? spawnerController.chargeTimesPerWave : new float[0];

            // 记录游戏数据
            if (FirebaseDataSender.Instance != null)
            {
                FirebaseDataSender.Instance.SendGameResult(isWin, currentWave, Time.timeSinceLevelLoad, flashlightDurations,
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

        // 防止多次触发
        this.enabled = false;
    }
}
