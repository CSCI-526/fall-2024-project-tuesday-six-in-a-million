using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
     public float energyAmount = 20f;  // Amount of energy recovered each time

    // Triggered when the player touches the object
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a player
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // Calls the player's energy recovery function
            // player.RechargeEnergy(energyAmount);

            // Objects picked up and destroyed
            Destroy(gameObject);
        }
    }
}
