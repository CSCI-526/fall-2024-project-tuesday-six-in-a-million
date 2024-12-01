using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController2 : MonoBehaviour
{
    public GameObject target;  // The target that the bullet will move toward
    public float speed = 10.0f;  // Speed of the bullet
    public float energyCost = 1f;  // Energy cost of the bullet
    public float goldSpawnOffset = 1f;  // Offset to spawn gold near the target
    public SpawnerController spawnerController;  // Reference to SpawnerController
    public GameObject goldPrefab;
    public TowerController originTower;  // Reference to the tower that shot the bullet

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (target == null)
        {
            Destroy(gameObject);  // Destroy the bullet if there's no target
            return;
        }

        // Move the bullet towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the bullet hits the target
        if (other.gameObject == target)
        {
            Debug.Log("Bullet hit the target!");
            ProcessTargetHit(other.gameObject);
        }
    }

    private void ProcessTargetHit(GameObject enemy)
    {
        // Destroy the bullet
        Destroy(gameObject);

        // Check for EnemyController or EnemyController2
        var enemyController = enemy.GetComponent<EnemyController>();
        var enemyController2 = enemy.GetComponent<EnemyController2>();

        if (enemyController != null)
        {
            ApplyDamageAndProcess(enemyController, enemy);
        }
        else if (enemyController2 != null)
        {
            ApplyDamageAndProcess(enemyController2, enemy);
        }
        else
        {
            Debug.LogError("No compatible EnemyController found on the target.");
        }
    }

    private void ApplyDamageAndProcess(MonoBehaviour enemyController, GameObject enemy)
    {
        int health = (enemyController is EnemyController)
            ? ((EnemyController)enemyController).health--
            : ((EnemyController2)enemyController).health--;

        Debug.Log($"Enemy health after hit: {health}");

        UpdateEnemyColor(enemyController);

        if (health <= 0)
        {
            KillEnemy(enemy);
        }
    }

    private void UpdateEnemyColor(MonoBehaviour enemyController)
    {
        Renderer renderer = enemyController.GetComponent<Renderer>();
        if (renderer != null)
        {
            int health = (enemyController is EnemyController)
                ? ((EnemyController)enemyController).health
                : ((EnemyController2)enemyController).health;

            if (health == 2)
                renderer.material.color = Color.yellow;
            else if (health == 1)
                renderer.material.color = Color.red;
        }
    }

    private void KillEnemy(GameObject enemy)
    {
        Debug.Log("Enemy has been killed.");

        // Increment kill counts
        if (spawnerController != null) spawnerController.totalEnemiesKilled++;
        if (originTower != null) originTower.totalKillCount++;

        Destroy(enemy);  // Destroy the enemy
        GenerateGold(enemy);  // Generate gold when the enemy is killed
    }

    private void GenerateGold(GameObject enemy)
    {
        // Define a random offset (only for X and Z axes)
        Vector3 randomOffset = new Vector3(
            Random.Range(-goldSpawnOffset, goldSpawnOffset), // Random offset on the X-axis
            0,                                              // Fixed height for the ground (Y-axis)
            Random.Range(-goldSpawnOffset, goldSpawnOffset)  // Random offset on the Z-axis
        );
        // Calculate the final spawn position
        Vector3 goldSpawnPosition = new Vector3(
            enemy.transform.position.x + randomOffset.x,  // Enemy's X position with random offset
            -0.5f,                                        // Fixed Y height for ground level
            enemy.transform.position.z + randomOffset.z   // Enemy's Z position with random offset
        );

        // Instantiate the gold prefab at the calculated position
        GameObject gold = Instantiate(goldPrefab, goldSpawnPosition, Quaternion.identity);
        gold.SetActive(true); // Ensure the gold object is active
    }
}
