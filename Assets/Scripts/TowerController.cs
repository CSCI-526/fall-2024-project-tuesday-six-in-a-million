using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public GameObject bulletPrefab;  // Bullet prefab to shoot
    public Transform firePoint;      // The position where bullets are fired from
    public float attackInterval = 2f;  // Time between each attack
    public bool isCharged = false;  // Whether the tower is charged
    private float attackTimer = 0f;  // Timer to control attack intervals
    public float range = 10f;       // Range within which the tower can attack enemies

    void Start()
    {
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned to " + gameObject.name);
        }

        attackTimer = attackInterval;  // Initialize the attack timer
    }

    void Update()
    {
        // Only allow the tower to attack if it has been charged
        if (isCharged && firePoint != null)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                FireBullet();
                attackTimer = attackInterval;  // Reset the attack timer
            }
        }
    }

    // Method to charge the tower using the flashlight
    public void ChargeTower()
    {
        isCharged = true;  // The tower is now charged and can fire bullets
        Debug.Log(gameObject.name + " is fully charged and ready to fire!");
    }

    // Fire bullets at the target
    void FireBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("No bullet prefab assigned to " + gameObject.name);
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.transform.localScale = bulletPrefab.transform.localScale;  // Ensure the bullet retains its correct scale

        // Find the nearest enemy and assign it as the bullet's target if within range
        BulletController bulletController = bullet.GetComponent<BulletController>();
        GameObject closestEnemy = FindClosestEnemy();

        if (closestEnemy != null && Vector3.Distance(transform.position, closestEnemy.transform.position) <= range)
        {
            bulletController.target = closestEnemy;  // Set the closest enemy as the target
        }
        else
        {
            Destroy(bullet);  // Destroy the bullet if no enemy is within range
            Debug.Log("No enemy in range, bullet destroyed.");
        }
    }

    // Find the closest enemy to target
    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = firePoint.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < minDistance)
            {
                closestEnemy = enemy;
                minDistance = distanceToEnemy;
            }
        }

        return closestEnemy;
    }
}
