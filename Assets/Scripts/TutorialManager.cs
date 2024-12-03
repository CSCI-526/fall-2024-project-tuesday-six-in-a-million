using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public FlashlightPowerUpdater flashlightPowerUpdater;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI batteryWarningText;
    public GameObject towerPlacementZoneLight;
    public GameObject enemySpawner;
    public GoldUpdater goldUpdater;
    public Transform playerTransform;
    public GameObject arrow;
    public Transform[] chargingStations;

    private bool towerPlaced = false;
    private bool batteryLow = false;
    private bool lightTowerPromptShown = false;
    private bool goldPromptActive = false;
    private bool towerCharged = false; // Track if the tower is charged
    private Vector3 currentTargetPosition; // Use Vector3 for positions instead of Transform
    private Vector3 targetZonePosition = new Vector3(-1.987482f, 1f, -15.99166f); // Coordinates for tower placement zone
    private float detectionRadius = 2f;
    private float lowBatteryThreshold = 20f;
    private float rechargeThreshold = 50f;
    private float arrowYOffset = 5f; // Increased arrow Y-offset to avoid overlapping tutorial text

    private void Start()
    {
        tutorialText.gameObject.SetActive(false);
        batteryWarningText.gameObject.SetActive(false);
        arrow.SetActive(false);
        StartCoroutine(TutorialSequence());
    }

    private void Update()
    {
        // Update the arrow's position and rotation to follow the player and point to the current target position
        if (arrow.activeSelf)
        {
            arrow.transform.position = playerTransform.position + Vector3.up * arrowYOffset; // Adjust height to avoid text overlap
            PointArrowAt(currentTargetPosition);
        }

        // Dynamically update the nearest charging station if the battery is low
        if (batteryLow)
        {
            Transform nearestStation = GetNearestChargingStation();
            if (nearestStation != null && nearestStation.position != currentTargetPosition)
            {
                currentTargetPosition = nearestStation.position;
            }
        }

        // Update battery warning text visibility based on flashlight power
        if (flashlightPowerUpdater.flashlightPower < lowBatteryThreshold && !batteryLow)
        {
            ShowBatteryWarning();
        }
        else if (flashlightPowerUpdater.flashlightPower >= rechargeThreshold && batteryLow)
        {
            HideBatteryWarning();
        }

        // Prompt the player to build a light tower when gold reaches 150
        if (goldUpdater.gold >= 150 && !lightTowerPromptShown)
        {
            ShowLightTowerPrompt();
        }
    }

    private IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(MoveToHighlightedZone());
        yield return StartCoroutine(TeachFlashlightUse());
        yield return StartCoroutine(PlaceTower());
        yield return StartCoroutine(ChargeTowerWithFlashlight());
        yield return StartCoroutine(CheckGoldCollection());
        StartWaveSpawning();
    }

    private IEnumerator MoveToHighlightedZone()
    {
        tutorialText.text = "Move to the highlighted yellow area.";
        tutorialText.gameObject.SetActive(true);
        arrow.SetActive(true);
        currentTargetPosition = targetZonePosition;

        while (Vector3.Distance(playerTransform.position, targetZonePosition) > detectionRadius)
        {
            yield return null;
        }

        arrow.SetActive(false);
        towerPlacementZoneLight.SetActive(false);
        tutorialText.gameObject.SetActive(false);
    }

    private IEnumerator TeachFlashlightUse()
    {
        tutorialText.text = "Press 'Left-Click' to turn ON your flashlight.";
        tutorialText.gameObject.SetActive(true);

        while (!flashlightPowerUpdater.flashlight.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                flashlightPowerUpdater.flashlight.enabled = true;
            }
            yield return null;
        }

        tutorialText.text = "Flashlight is ON! Great job!";
        yield return new WaitForSeconds(2);
        tutorialText.gameObject.SetActive(false);
    }

    private IEnumerator PlaceTower()
    {
        tutorialText.text = "Press 'Left Shift + Q' to place a tower.";
        tutorialText.gameObject.SetActive(true);

        while (!towerPlaced)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
            {
                towerPlaced = true;
                SpawnSingleEnemy(); // Spawn a single enemy after tower placement
            }
            yield return null;
        }

        tutorialText.text = "Tower placed! Use your flashlight to charge it.";
        yield return new WaitForSeconds(2);
        tutorialText.gameObject.SetActive(false);
    }

    private IEnumerator ChargeTowerWithFlashlight()
    {
        tutorialText.text = "Use 'Left-Click' to shine your flashlight on the tower and charge it.";
        tutorialText.gameObject.SetActive(true);

        while (!towerCharged)
        {
            // Logic to check if the tower is fully charged
            if (towerPlaced && flashlightPowerUpdater.flashlight.enabled)
            {
                // Add your logic to determine when the tower is fully charged (e.g., after a certain time or power threshold)
                towerCharged = true;
            }
            yield return null;
        }

        tutorialText.text = "Tower charged! Excellent!";
        yield return new WaitForSeconds(2);
        tutorialText.gameObject.SetActive(false);
    }

    private IEnumerator CheckGoldCollection()
    {
        while (true)
        {
            if (goldUpdater.gold > 0 && !goldPromptActive)
            {
                tutorialText.text = "Collect gold dropped by defeated enemies!";
                tutorialText.gameObject.SetActive(true);
                goldPromptActive = true;
            }

            if (goldUpdater.gold >= 20) // Hide prompt after collecting gold
            {
                tutorialText.gameObject.SetActive(false);
                break;
            }

            yield return null;
        }
    }

    private void ShowBatteryWarning()
    {
        batteryLow = true;
        batteryWarningText.text = "Your flashlight is low. Go to the nearest charging station.";
        batteryWarningText.gameObject.SetActive(true);

        arrow.SetActive(true);
        currentTargetPosition = GetNearestChargingStation().position;
    }

    private void HideBatteryWarning()
    {
        batteryLow = false;
        batteryWarningText.gameObject.SetActive(false);
        arrow.SetActive(false);
    }

    private Transform GetNearestChargingStation()
    {
        Transform nearestStation = null;
        float shortestDistance = float.MaxValue;

        foreach (Transform station in chargingStations)
        {
            float distance = Vector3.Distance(playerTransform.position, station.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestStation = station;
            }
        }

        return nearestStation;
    }

    private void ShowLightTowerPrompt()
    {
        lightTowerPromptShown = true;
        tutorialText.text = "You have enough gold! Build a light tower near the normal tower (Press Left Shift + E).";
        tutorialText.gameObject.SetActive(true);

        arrow.SetActive(true);
        currentTargetPosition = playerTransform.position;

        Invoke("HideLightTowerPrompt", 10);
    }

    private void HideLightTowerPrompt()
    {
        tutorialText.gameObject.SetActive(false);
        arrow.SetActive(false);
    }

    private void PointArrowAt(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - arrow.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, rotation, Time.deltaTime * 5f);
    }

    private void SpawnSingleEnemy()
    {
        if (enemySpawner != null)
        {
            SpawnerController spawnerController = enemySpawner.GetComponent<SpawnerController>();
            if (spawnerController != null)
            {
                spawnerController.SpawnSingleEnemy();
            }
        }
    }

    private void StartWaveSpawning()
    {
        if (enemySpawner != null)
        {
            SpawnerController spawnerController = enemySpawner.GetComponent<SpawnerController>();
            if (spawnerController != null)
            {
                spawnerController.CompleteTutorial();
            }
        }
    }
}
