using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject target;  // The target that the bullet will move toward
    public float speed = 10.0f;  // Speed of the bullet

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
                Destroy(target);  // Destroy the target when health is 0
            }
        }
    }
}
