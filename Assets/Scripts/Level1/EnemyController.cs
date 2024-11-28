using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int reward = 50;
    public int health = 1;
    public int moveSpeed = 5;
    public GameObject Base1;
    public GameObject Base2;
    public GameObject Player;
    public GameObject ResetButton;
    public GameObject LevelSelectButton;
    public Light flashlight;
    public FlashlightCollider flashlightCollider;
    public FlashlightPowerUpdater flashlightPowerUpdater;
    public FirebaseDataSender firebaseDataSender;  // Reference to FirebaseDataSender
    public SpawnerController spawnerController; // Reference to SpawnerController  

    private Renderer enemyRenderer;
    public Color highlightColor = Color.magenta; // Color to highlight enemies

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        
        // Set the color of the enemy based on health
        if (health == 3)
        {
            enemyRenderer.material.color = Color.green;
        }
        else if (health == 2)
        {
            enemyRenderer.material.color = Color.yellow;
        }
        else if (health == 1)
        {
            enemyRenderer.material.color = Color.red;
        }

        // Highlight enemy upon spawning
        HighlightEnemy();
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        // Move the enemy towards the closest base
        GameObject Base = Base1;
        if (Vector3.Distance(transform.position, Base1.transform.position) > Vector3.Distance(transform.position, Base2.transform.position))
        {
            Base = Base2;
        }
        int moveDistance = moveSpeed;
        if (flashlightCollider.IsHitByFlashlight(gameObject))
        {
            moveDistance = moveSpeed / 2;
        }
        transform.position = Vector3.MoveTowards(transform.position, Base.transform.position, moveDistance * Time.deltaTime);
    }

    // Method to permanently highlight the enemy
    public void HighlightEnemy()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = highlightColor;  // Set highlight color
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the enemy hits the base, destroy the enemy
        if (collision.gameObject == Base1 || collision.gameObject == Base2 || collision.gameObject == Player)
        {
            // Stop the game and display game over UI
            Time.timeScale = 0;
            GameObject.Find("GameOver").GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ResetButton.SetActive(true);
            LevelSelectButton.SetActive(true);

            List<TowerData> towerDataList = new List<TowerData>();
            if (spawnerController != null) 
            {
                List<TowerController> allTowers = spawnerController.allTowers;
                Debug.Log("Number of Tower" + allTowers.Count);
                foreach (TowerController tower in allTowers)
                {
                    TowerData data = new TowerData
                    {
                        totalChargeTime = tower.totalChargeTime,
                        totalKillCount = tower.totalKillCount
                    };
                    towerDataList.Add(data);
                    Debug.Log("Tower Charging Time：" + data.totalChargeTime + ", Kill Count" + data.totalKillCount);
                }
            } 
            else
            {
                Debug.LogError("SpawnerController reference is missing in EnemyController.");
            }

            flashlightPowerUpdater.AddDuration();
            List<float> flashlightDurations = flashlightPowerUpdater.GetUsageDurations();
            int currentWave = FindObjectOfType<SpawnerController>().currentWave;
            float[] chargeTimesPerWave = FindObjectOfType<SpawnerController>().chargeTimesPerWave;
            FirebaseDataSender.Instance.SendGameResult(1, false, currentWave, Time.timeSinceLevelLoad, flashlightDurations,
             towerDataList, chargeTimesPerWave);

            
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.GameOver();
            }

            this.enabled = false;
        }
    }
}
