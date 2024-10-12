using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightPowerUpdater : MonoBehaviour
{
    public Text flashlightPowerText;  // UI Text to display the flashlight power

    // Function to update the UI with the current flashlight power
    public void UpdateFlashlightUI(float power)
    {
        flashlightPowerText.text = "FlashLightPower: " + Mathf.FloorToInt(power).ToString();
    }
}
