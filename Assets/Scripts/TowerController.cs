using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : MonoBehaviour
{
    public GameObject bulletPrefab;  // Bullet prefab to shoot
    public GameObject Flashlight;    // Reference to the flashlight object
    public GameObject Indicator;     // Reference to the indicator object
    public Transform firePoint;      // The position where bullets are fired from
    public float attackInterval = 2f;  // Time between each attack
    public float chargeLevel = 0f;    // The charge level of the tower
    public float maxChargeLevel = 10f;  // The maximum charge level of the tower
    private float attackTimer = 0f;  // Timer to control attack intervals
    public float range = 10f;       // Range within which the tower can attack enemies

    void Start()
    {
        // Create an indicator to show tower charge level
        Indicator = Instantiate(Indicator, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        // Set the scale to be 0.1
        Indicator.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned to " + gameObject.name);
        }

        attackTimer = attackInterval;  // Initialize the attack timer
    }

    void Update()
    {
        // Only allow the tower to attack if it has been charged
        float bulletEnergyCost = bulletPrefab.GetComponent<BulletController>().energyCost;
        attackTimer -= Time.deltaTime;
        if (firePoint != null && chargeLevel >= bulletEnergyCost)
        {
            if (attackTimer <= 0f)
            {
                FireBullet();
                chargeLevel -= bulletEnergyCost;  // Deduct energy cost from the charge level
                attackTimer = attackInterval;  // Reset the attack timer
            }
        }

        // Change the text to show the charge level
        Text chargeText = Indicator.GetComponentInChildren<UnityEngine.UI.Text>();
        chargeText.text = chargeLevel.ToString("F2");
        // Set the canvas to face away from the camera
        Indicator.transform.LookAt(Indicator.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    // Method to charge the tower using the flashlight
    public void ChargeTower()
    {
        if (chargeLevel < maxChargeLevel)
        {
            chargeLevel += Flashlight.GetComponent<PlayerController>().powerDrainAmount * Time.deltaTime;
            if (chargeLevel > maxChargeLevel)
            {
                chargeLevel = maxChargeLevel;  // Clamp the charge level to the maximum
            }
            Debug.Log("Tower charged by " + Flashlight.GetComponent<PlayerController>().powerDrainAmount + " energy.");
        }
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
            chargeLevel += bulletController.energyCost;  // Refund the energy cost
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
