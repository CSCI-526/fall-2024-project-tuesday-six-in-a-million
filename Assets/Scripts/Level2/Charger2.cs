using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger2 : MonoBehaviour
{
    public FlashlightPowerUpdater flashlight;
    public SecondLevelSpawnerController spawnerController; // Reference to the SecondLevelSpawnerController script

    void Start()
    {
        if (spawnerController == null)
        {
            spawnerController = FindObjectOfType<SecondLevelSpawnerController>();
            if (spawnerController == null)
            {
                Debug.LogError("SecondLevelSpawnerController not found! Please set it in the Inspector.");
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision Stay");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Collision Stay");
            flashlight.Charge();

            // Update charging time in SecondLevelSpawnerController
            if (spawnerController != null && spawnerController.currentWave > 0)
            {
                spawnerController.AddChargeTime(Time.deltaTime);
            }
            else
            {
                Debug.LogWarning("Cannot update charging time in SecondLevelSpawnerController");
            }
        }
    }
}
