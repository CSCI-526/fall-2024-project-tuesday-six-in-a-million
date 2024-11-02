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
    public TowerController originTower; // reference the tower shoot the bullet 

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
        // If the bullet hits the target, destroy the bullet and deal damage to the target
        if (collision.gameObject == target)
        {
            Destroy(gameObject);  // Destroy the bullet
            target.GetComponent<EnemyController>().health--;

            // Change the color of the target based on health
            if (target.GetComponent<EnemyController>().health == 2)
            {
                target.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else if (target.GetComponent<EnemyController>().health == 1)
            {
                target.GetComponent<Renderer>().material.color = Color.red;
            }
            else if (target.GetComponent<EnemyController>().health <= 0)
            {   
                if (spawnerController != null)
                {
                    spawnerController.totalEnemiesKilled++;
                }

                if (originTower != null)
                {
                    originTower.totalKillCount++; //increase tower kill count
                }

                Destroy(target);  // Destroy the target when health is 0
                GenerateGold();  // Generate gold when the enemy is killed
            }
        }
    }

    void GenerateGold()
    {
        // pick a random location near the target
        Vector3 randomOffset = new Vector3(Random.Range(-goldSpawnOffset, goldSpawnOffset), 0, Random.Range(-goldSpawnOffset, goldSpawnOffset));
        // Generate gold when the enemy is killed
        GameObject gold = Instantiate(goldPrefab, target.transform.position + randomOffset, Quaternion.identity);
        // make the gold object active
        gold.SetActive(true);
    }
}
