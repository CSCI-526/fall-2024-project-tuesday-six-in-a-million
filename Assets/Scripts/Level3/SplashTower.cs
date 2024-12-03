using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashTower : MonoBehaviour
{
    [Header("Tower Settings")]
    public float attackRadius = 5f; // The radius of the splash damage
    public float attackCooldown = 2f; // Time between attacks
    public float damage = 50f; // Damage dealt to each enemy

    [Header("Visual Effects")]
    public GameObject splashEffect; // Optional: Add a splash effect (e.g., explosion animation)

    private float nextAttackTime;

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        // Find all colliders within the attack radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Enemy")) // Ensure it targets enemies only
            {
                // Apply damage to the enemy
                EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }

        // Optional: Play splash effect
        if (splashEffect != null)
        {
            Instantiate(splashEffect, transform.position, Quaternion.identity);
        }
    }

    // Debugging: Draw attack radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
