using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarBullet : MonoBehaviour
{
    public Vector3 midPos;
    private bool reachMidPos = false;
    public Vector3 endPos;
    private bool reachEndPos = false;
    public float bulletSpeed = 100f;
    public float damage = 10f;
    public float damageRadius = 20f;
    public GoldUpdater goldUpdater;
    void Start()
    {
        
    }

    
    void Update()
    {
        if (midPos != null && Vector3.Distance(transform.position,midPos) > 5f && !reachMidPos)
        {
           transform.LookAt(midPos);
           transform.position += transform.forward * bulletSpeed * Time.deltaTime;

        }
        if (midPos != null && Vector3.Distance(transform.position,midPos) <= 5f && !reachMidPos)
        {
            reachMidPos = true;
        }

        if (reachMidPos && endPos != null && Vector3.Distance(transform.position, endPos) > 5f && !reachEndPos)
        {
            transform.LookAt(endPos);
            transform.position += transform.forward * bulletSpeed * Time.deltaTime;

        }

        if (reachMidPos && endPos != null && Vector3.Distance(transform.position, endPos) <= 5f && !reachEndPos)
        {
            reachEndPos = true;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Vector3 currentPosition = transform.position;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
                Debug.Log($"Checking enemy: {enemy.name}, Distance: {distanceToEnemy}");

                if (distanceToEnemy < damageRadius)
                {
                    goldUpdater.AddGold(20);
                    Debug.Log($"Enemy {enemy.name} within range, dealing damage.");
                    Destroy(enemy.gameObject);
                   // enemy.GetComponent<EnemyController>().TakeDamage(damage);
               
                }
            }

            Destroy(gameObject);

        }

    }
}
