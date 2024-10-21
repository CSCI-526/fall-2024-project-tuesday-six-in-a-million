using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int goldAmount = 10;  // Amount of gold to give the player
    public GoldUpdater goldUpdater;  // Reference to the UI updater script

    // Triggered when the player touches the object
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a player
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // Calls the player's energy recovery function
            goldUpdater.AddGold(goldAmount);

            // Objects picked up and destroyed
            Destroy(gameObject);
        }
    }
}
