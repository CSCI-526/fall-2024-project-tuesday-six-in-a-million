using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{
    // Start is called before the first frame update
    public FlashlightPowerUpdater flashlight;
    void Start()
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision Stay");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Collision Stay");
            flashlight.Charge();
        } 
    }
}
