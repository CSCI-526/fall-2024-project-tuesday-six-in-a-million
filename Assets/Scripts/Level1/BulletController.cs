using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject target;  // The target that the bullet will move toward
    public float speed = 10.0f;  // Speed of the bullet
    public float energyCost = 1f;  // Energy cost of the bullet
    public float goldSpawnOffset = 1f;  // Offset to spawn gold near the target
    public SpawnerController spawnerController;  // Reference to SpawnerController for Level 1
    public SecondLevelSpawnerController secondLevelSpawnerController;  // Reference to SecondLevelSpawnerController for Level 2
    public GameObject goldPrefab;  // Prefab for gold to spawn
    public TowerController originTower;  // Reference to the tower that shot the bullet

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (target == null)
        {
            Debug.Log("No target assigned, bullet destroyed.");
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        // Raycast for collision detection
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distanceThisFrame))
        {
            Debug.Log($"Bullet hit: {hit.collider.gameObject.name}");
            if (hit.collider.gameObject == target)
            {
                ProcessCollision(hit.collider); // Process collision directly
                return;
            }
        }

        // Move the bullet
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, distanceThisFrame);
    }

    void ProcessCollision(Collider collider)
    {
        Debug.Log($"Bullet collided with: {collider.gameObject.name}");
        if (collider.gameObject == target)
        {
            Debug.Log("Bullet hit the target!");
            Destroy(gameObject);

            // Check for EnemyController or EnemyController2 on the target
            var enemyController = target.GetComponent<EnemyController>();
            var enemyController2 = target.GetComponent<EnemyController2>();

            if (enemyController != null)
            {
                ProcessEnemyHit(enemyController);
            }
            else if (enemyController2 != null)
            {
                ProcessEnemyHit(enemyController2);
            }
            else
            {
                Debug.LogError("No compatible EnemyController found on the target.");
            }
        }
    }

    private void ProcessEnemyHit(EnemyController enemy)
    {
        enemy.health--;
        Debug.Log($"Enemy health after hit: {enemy.health}");

        if (enemy.health <= 0)
        {
            KillEnemy(enemy.gameObject);
        }
    }

    private void ProcessEnemyHit(EnemyController2 enemy)
    {
        enemy.health--;
        Debug.Log($"Enemy health after hit: {enemy.health}");

        if (enemy.health <= 0)
        {
            KillEnemy(enemy.gameObject);
        }
    }

    private void KillEnemy(GameObject enemy)
    {
        Debug.Log("Enemy has died.");

        // Update enemy kill count in the respective spawner controller
        if (spawnerController != null)
        {
            spawnerController.totalEnemiesKilled++;
        }

        if (secondLevelSpawnerController != null)
        {
            secondLevelSpawnerController.totalEnemiesKilled++;
        }

        // Update kill count in the tower
        if (originTower != null)
        {
            originTower.totalKillCount++;
        }

        Destroy(enemy);  // Destroy the enemy
        GenerateGold(enemy);  // Generate gold when the enemy is killed
    }

    private void GenerateGold(GameObject enemy)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-goldSpawnOffset, goldSpawnOffset), // Random offset on the X-axis
            0,                                              // Fixed height for the ground (Y-axis)
            Random.Range(-goldSpawnOffset, goldSpawnOffset)  // Random offset on the Z-axis
        );
        // Calculate the final spawn position
        Vector3 goldSpawnPosition = new Vector3(
            enemy.transform.position.x + randomOffset.x,  // Enemy's X position with random offset
            1,
            enemy.transform.position.z + randomOffset.z   // Enemy's Z position with random offset
        );

        // Instantiate the gold prefab at the calculated position
        GameObject gold = Instantiate(goldPrefab, goldSpawnPosition, Quaternion.identity);
        gold.SetActive(true);
        Debug.Log("Gold generated near enemy's position.");
    }
}
