using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    public int goldCost = 50;
    public float attackDuration = 1f;
    public float bulletMaxH = 10f;
    private float attackTimer = 0f;
    private Vector3 midPos = Vector3.zero;
    public GameObject bulletPref;
    public Transform firePoint;
    public GoldUpdater goldUpdater;
    void Start()
    {
        
    }

    
    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackDuration)
        { 
            GameObject enemy = FindClosestEnemy();
            if (enemy != null)
            {
                attackTimer = 0f;
                midPos = (firePoint.position + firePoint.position) / 2 + new Vector3(0,bulletMaxH,0);
                GameObject bullet = Instantiate(bulletPref, firePoint.position, firePoint.rotation);
                bullet.GetComponent<MortarBullet>().midPos = midPos;
                bullet.GetComponent<MortarBullet>().endPos = enemy.transform.position;
                bullet.GetComponent<MortarBullet>().goldUpdater = goldUpdater;
            }

        }
        
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = 100;
        Vector3 currentPosition = firePoint.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            Debug.Log($"Checking enemy: {enemy.name}, Distance: {distanceToEnemy}");

            if (distanceToEnemy < minDistance)
            {
                closestEnemy = enemy;
                minDistance = distanceToEnemy;
            }
        }

        if (closestEnemy != null)
            Debug.Log($"Closest enemy: {closestEnemy.name}, Distance: {minDistance}");
        else
            Debug.Log("No enemies found in range.");

        return closestEnemy;
    }
}

