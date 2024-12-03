using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightPowerUpdater : MonoBehaviour
{
    public Text flashlightPowerText;  // UI Text to display the flashlight power
    public Light flashlight;          // The flashlight object
    public float flashlightMaxPower = 100f; // Flashlight's maximum power
    public float flashlightPower = 50f;     // Flashlight's current power
    public float powerRechargeRate = 10f;  // How fast flashlight recharges when off
    public float powerDrainRate = 5f;     // How fast flashlight drains when on
    public float flashlightRange = 30f;   // Range of flashlight to interact with towers
    private float usageStartTime = 0f;
    private List<float> usageDurations = new List<float>();
    
    void Start()
    {
        UpdateFlashlightUI();
    }

    // Function to update the UI with the current flashlight power
    public void UpdateFlashlightUI()
    {
        flashlightPowerText.text = "FlashLightPower: " + Mathf.FloorToInt(flashlightPower).ToString();
    }

    public void toggleFlashlight()
    {
        if (!flashlight.enabled && flashlightPower > 0)
        {
            flashlight.enabled = true;
            usageStartTime = Time.time; // Start tracking duration

        }
        else
        {
            flashlight.enabled = false;
            float duration = Time.time - usageStartTime; // Calculate usage duration and add to list
            usageDurations.Add(duration);
        }
    }

    void Update() {
        if (!flashlight.enabled) {
            return;
        }
        flashlightPower -= powerDrainRate * Time.deltaTime;
        if (flashlightPower <= 0) {
            flashlightPower = 0;
            flashlight.enabled = false;
            float duration = Time.time - usageStartTime; // Calculate usage duration and add to list
            usageDurations.Add(duration);
        }
        UpdateFlashlightUI();
    }

    public void Charge() {
        flashlightPower += powerRechargeRate * Time.deltaTime;
        if (flashlightPower > flashlightMaxPower) {
            flashlightPower = flashlightMaxPower;
        }
        UpdateFlashlightUI();
    }


    public List<float> GetUsageDurations()
    {
        return usageDurations;
    }

    
    public void  AddDuration()
    {
        float duration = Time.time - usageStartTime; // Calculate usage duration and add to list
        usageDurations.Add(duration);
    }
    
}
