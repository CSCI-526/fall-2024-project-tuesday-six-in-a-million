using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : MonoBehaviour
{
    public GameObject bulletPrefab;  // Bullet prefab to shoot
    public GameObject Indicator;     // Reference to the indicator object
    public Transform firePoint;      // The position where bullets are fired from
    public int goldCost = 50;        // The cost of the tower
    public float attackInterval = 2f;  // Time between each attack
    public float chargeLevel = 0f;    // The charge level of the tower
    public float maxChargeLevel = 10f;  // The maximum charge level of the tower
    private float attackTimer = 0f;  // Timer to control attack intervals
    public float range = 10f;       // Range within which the tower can attack enemies
    public FlashlightCollider flashlightCollider;  // Reference to the flashlight collider script
    public FlashlightPowerUpdater flashlight;  // Reference to the flashlight power updater script

    public float chargingFrequency = 0f;   // Charging Frequency
    public int totalKillCount = 0;   // Total KillCount

    private float lastChargeTime = 0f;  // lastChargeTime
    public float totalChargeTime = 0f; // totalChargeTime
    private int chargeEvents = 0;       // count charge times

    void Start()
    {
        if (Indicator != null)
        {
            Indicator = Instantiate(Indicator, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            Indicator.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
        else
        {
            Debug.LogWarning("Indicator prefab is not assigned.");
        }

        if (firePoint == null)
        {
            Debug.LogError($"FirePoint is not assigned to {gameObject.name}.");
        }

        attackTimer = attackInterval;
        lastChargeTime = Time.time;
    }

    void Update()
    {
        if (Indicator != null)
        {
            Indicator.transform.position = transform.position + new Vector3(0, 2, 0);
        }

        if (flashlightCollider != null && flashlightCollider.IsHitByFlashlight(gameObject))
        {
            ChargeTower();
        }

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"bulletPrefab or firePoint is missing in {gameObject.name}.");
            return;
        }

        float bulletEnergyCost = bulletPrefab.GetComponent<BulletController>().energyCost;
        attackTimer -= Time.deltaTime;

        if (chargeLevel >= bulletEnergyCost)
        {
            if (attackTimer <= 0f)
            {
                FireBullet();
                chargeLevel -= bulletEnergyCost;  // Deduct energy cost from the charge level
                attackTimer = attackInterval;    // Reset the attack timer
            }
        }

        if (Indicator != null)
        {
            Text chargeText = Indicator.GetComponentInChildren<Text>();
            if (chargeText != null)
            {
                chargeText.text = chargeLevel.ToString("F2");
                Indicator.transform.LookAt(Indicator.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            }
        }
    }

    public void ChargeTower()
    {
        if (flashlight == null)
        {
            Debug.LogWarning($"Flashlight reference is missing in {gameObject.name}.");
            return;
        }

        ChargeTowerCustomSpeed(flashlight.powerDrainRate * 0.2f);
        float currentTime = Time.time;
        float deltaTime = currentTime - lastChargeTime;
        totalChargeTime += deltaTime;
        chargeEvents++;
        chargingFrequency = chargeEvents / totalChargeTime;
        lastChargeTime = currentTime;
    }

    public void ChargeTowerCustomSpeed(float chargeSpeed)
    {
        if (chargeLevel < maxChargeLevel)
        {
            chargeLevel += chargeSpeed * Time.deltaTime;
            if (chargeLevel > maxChargeLevel)
            {
                chargeLevel = maxChargeLevel;
            }
        }
    }

    void FireBullet()
    {
        Debug.Log($"Attempting to fire bullet from {gameObject.name}");

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"Cannot fire bullet: Missing bulletPrefab or firePoint in {gameObject.name}.");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.SetActive(true);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (bulletController == null)
        {
            Debug.LogError("BulletController component is missing on bulletPrefab.");
            Destroy(bullet);
            return;
        }

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null && Vector3.Distance(transform.position, closestEnemy.transform.position) <= range)
        {
            bulletController.target = closestEnemy;  // Set the closest enemy as the target
            bulletController.originTower = this;    // Set reference to this tower
            Debug.Log($"Bullet fired at enemy: {closestEnemy.name}");
        }
        else
        {
            Debug.Log("No enemy in range, bullet destroyed.");
            Destroy(bullet);
            chargeLevel += bulletController.energyCost;  // Refund the energy cost
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;
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
