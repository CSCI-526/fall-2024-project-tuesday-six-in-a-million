using System.Collections;
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
        }
        else
        {
            flashlight.enabled = false;
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
}
