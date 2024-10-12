using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemyPrefab;
    public float enemyInterval = 1.0f;
    private float lastTime = -999;
    public int spawnCount = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        // Every interval, spawn an enemy
        if (Time.time - lastTime > enemyInterval)
        {
            spawnCount++;
            lastTime = Time.time;
            GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            enemy.GetComponent<EnemyController>().isPrefab = false;
            if (spawnCount > 8) {
                enemy.GetComponent<EnemyController>().health = 3;
            } else if (spawnCount > 5) {
                enemy.GetComponent<EnemyController>().health = 2;
            }
        }
    }
}
