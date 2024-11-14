using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject target;  // The target that the bullet will move toward
    public float speed = 10.0f;  // Speed of the bullet
    public float energyCost = 1f;  // Energy cost of the bullet
    public float goldSpawnOffset = 1f;  // Offset to spawn gold near the target
    public SpawnerController spawnerController;  // reference SpawnerController
    public GameObject goldPrefab;
    public TowerController originTower; // reference the tower that shot the bullet

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);  // Destroy the bullet if there's no target
            return;
        }

        // Move the bullet towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision object is the target
        if (collision.gameObject == target)
        {
            Debug.Log("Bullet hit the target!");

            Destroy(gameObject);  // Destroy the bullet

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
        Debug.Log("Enemy health after hit: " + enemy.health);

        UpdateEnemyColor(enemy);

        if (enemy.health <= 0)
        {
            KillEnemy(enemy.gameObject);
        }
    }

    private void ProcessEnemyHit(EnemyController2 enemy)
    {
        enemy.health--;
        Debug.Log("Enemy health after hit: " + enemy.health);

        UpdateEnemyColor(enemy);

        if (enemy.health <= 0)
        {
            KillEnemy(enemy.gameObject);
        }
    }

    private void UpdateEnemyColor(MonoBehaviour enemy)
    {
        // Update color based on health
        Renderer renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            int health = (enemy is EnemyController) ? ((EnemyController)enemy).health : ((EnemyController2)enemy).health;
            if (health == 2)
            {
                renderer.material.color = Color.yellow;
            }
            else if (health == 1)
            {
                renderer.material.color = Color.red;
            }
        }
    }

    private void KillEnemy(GameObject enemy)
    {
        Debug.Log("Enemy has died.");

        if (spawnerController != null)
        {
            spawnerController.totalEnemiesKilled++;
        }

        if (originTower != null)
        {
            originTower.totalKillCount++; // Increase tower kill count
        }

        Destroy(enemy);  // Destroy the enemy
        GenerateGold(enemy);  // Generate gold when the enemy is killed
    }

    private void GenerateGold(GameObject enemy)
    {
        // Pick a random location near the enemy
        Vector3 randomOffset = new Vector3(Random.Range(-goldSpawnOffset, goldSpawnOffset), 0, Random.Range(-goldSpawnOffset, goldSpawnOffset));
        // Generate gold when the enemy is killed
        GameObject gold = Instantiate(goldPrefab, enemy.transform.position + randomOffset, Quaternion.identity);
        gold.SetActive(true);
    }
}
