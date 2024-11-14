using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger1 : MonoBehaviour
{
    // Start is called before the first frame update
    public FlashlightPowerUpdater flashlight;
    
    public SpawnerController spawnerController; // Reference to the SpawnerController script
    void Start()
    {
        if (spawnerController == null)
        {
            spawnerController = FindObjectOfType<SpawnerController>();
            if (spawnerController == null)
            {
                Debug.LogError("SpawnerController not found! please set in Inspector。");
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision Stay");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Collision Stay");
            flashlight.Charge();

            // update charging time SpawnerController
            if (spawnerController != null && spawnerController.currentWave > 0)
            {
                spawnerController.AddChargeTime(Time.deltaTime);
            }
            else
            {
                Debug.LogWarning("can't update charging time int SpawnerController ");
            }
        } 
    }
}
