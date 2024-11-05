using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public FlashlightPowerUpdater flashlightPowerUpdater; // Reference to FlashlightPowerUpdater
    public TextMeshProUGUI tutorialText;                  // Tutorial text display
    public TextMeshProUGUI batteryWarningText;            // Battery warning text display
    public GameObject welcome;                            // Welcome message to "Defend Your Base"
    public GameObject towerPlacementZoneLight;            // Reference to Tower Placement Zone light object
    public GameObject enemySpawner;                       // Reference to the enemy spawner to control enemy spawning
    public GoldUpdater goldUpdater;                       // Reference to the gold updater
    public Transform playerTransform;                     // Reference to the player's transform

    private bool batteryWarningShown = false;
    private bool playerReachedZone = false;
    private bool lightTowerPromptShown = false;           // Track if light tower message has been shown

    // Coordinates of the spotlight for tower placement zone
    private Vector3 targetZonePosition = new Vector3(-1.987482f, 1f, -15.99166f); // Specified coordinates
    private float detectionRadius = 2f;  // Radius within which player needs to be to "reach" the zone

    private void Start()
    {
        Debug.Log("TutorialManager Start() called"); // Debug log to confirm Start is called

        if (flashlightPowerUpdater == null)
        {
            flashlightPowerUpdater = FindObjectOfType<FlashlightPowerUpdater>();
        }

        if (welcome != null) welcome.SetActive(false);
        tutorialText.gameObject.SetActive(false);
        batteryWarningText.gameObject.SetActive(false);

        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        Debug.Log("TutorialSequence started"); // Confirm that the sequence starts

        // Step 1: 3-second map exploration period
        yield return StartCoroutine(IntroductionPeriod());
        yield return new WaitForSeconds(1); // Add delay between messages

        // Step 2: Teach player to use flashlight
        yield return StartCoroutine(TeachFlashlightUse());
        yield return new WaitForSeconds(1); // Add delay between messages

        // Step 3: Guide player to tower placement zone
        yield return StartCoroutine(GuideToPlacementZone());
        yield return new WaitForSeconds(1); // Add delay between messages

        // Step 4: Spawn one enemy and prompt to place a tower
        SpawnSingleEnemy();
        yield return new WaitForSeconds(1); // Add delay between messages

        // Step 5: Start monitoring battery level separately (independent of tutorial steps)
        StartCoroutine(CheckBatteryLevel());

        // Step 6: Show "Defend Your Base!" message after tutorial
        yield return StartCoroutine(ShowFinalMessage());

        // Step 7: Start enemy waves after tutorial
        StartCoroutine(StartWaveSpawning());
    }

    private IEnumerator IntroductionPeriod()
    {
        tutorialText.text = "Use Left-click to toggle your flashlight and explore.";
        tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3); // Allow exploration for 3 seconds
        tutorialText.gameObject.SetActive(false);
    }

    private IEnumerator TeachFlashlightUse()
    {
        tutorialText.text = "Left-click to toggle your flashlight.";
        tutorialText.gameObject.SetActive(true);

        while (!flashlightPowerUpdater.flashlight.enabled)
        {
            yield return null;
        }

        tutorialText.gameObject.SetActive(false); // Hide tutorial text
    }

    private IEnumerator GuideToPlacementZone()
    {
        tutorialText.text = "Move to the highlighted area to place your tower.";
        tutorialText.gameObject.SetActive(true);

        while (Vector3.Distance(playerTransform.position, targetZonePosition) > detectionRadius)
        {
            yield return null; // Wait until player is within the radius of the target zone
        }

        // Player has reached the area, turn off spotlight and proceed
        if (towerPlacementZoneLight != null)
        {
            towerPlacementZoneLight.GetComponent<Light>().enabled = false; // Turn off spotlight
        }

        tutorialText.gameObject.SetActive(false); // Hide the tutorial text
    }

    private void SpawnSingleEnemy()
    {
        Debug.Log("Spawning single enemy"); // Confirm enemy spawn

        // Detect Left Shift + Q to place the shooting tower
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Left Shift + Q detected"); // Confirm the key combination was detected

            SpawnShootingTower(); // Call the function to spawn the tower
            tutorialText.text = "A shooting tower has been placed. Use the flashlight to charge it!";
        }
        else
        {
            // Prompt player to press Left Shift + Q if they haven’t already
            tutorialText.text = "Press Left Shift + Q to place a shooting tower. Use the flashlight to charge it!";
        }

        // Spawn a single enemy if enemySpawner exists
        if (enemySpawner != null)
        {
            enemySpawner.GetComponent<SpawnerController>().SpawnSingleEnemy();
        }

        // Display message and hide after 5 seconds
        tutorialText.gameObject.SetActive(true);
        Invoke("HideTutorialText", 5); // Hide message after 5 seconds
    }

    private void SpawnShootingTower()
    {
        Debug.Log("Spawning shooting tower"); // Confirm tower spawn
        // Add your tower spawning logic here
    }

    private IEnumerator CheckBatteryLevel()
    {
        while (true)
        {
            if (flashlightPowerUpdater.flashlightPower < 20 && !batteryWarningShown)
            {
                ShowBatteryWarning();
            }

            if (goldUpdater.gold >= 150 && !lightTowerPromptShown)
            {
                ShowLightTowerHint();
            }

            yield return new WaitForSeconds(1); // Check battery level and gold every second
        }
    }

    private void ShowBatteryWarning()
    {
        batteryWarningShown = true;
        batteryWarningText.text = "Low battery! Go to the charging station.";
        batteryWarningText.gameObject.SetActive(true);
        Invoke("HideBatteryWarning", 5); // Hide after 5 seconds
    }

    private void HideBatteryWarning()
    {
        batteryWarningText.gameObject.SetActive(false);
    }

    private void ShowLightTowerHint()
    {
        lightTowerPromptShown = true;
        tutorialText.text = "You have enough gold! Press Left Shift + E to place a light tower to recharge other towers.";
        tutorialText.gameObject.SetActive(true);
        Invoke("HideTutorialText", 5); // Hide after 5 seconds
    }

    private IEnumerator ShowFinalMessage()
    {
        yield return new WaitForSeconds(2); // Brief pause before showing final message
        tutorialText.text = "Remember to use Light Towers to recharge other towers!";
        tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5); // Show message for 5 seconds
        tutorialText.gameObject.SetActive(false);

        if (welcome != null)
        {
            welcome.SetActive(true);
        }
    }

    private void HideTutorialText()
    {
        tutorialText.gameObject.SetActive(false);
    }

    private IEnumerator StartWaveSpawning()
    {
        yield return new WaitForSeconds(3); // Delay before starting the first wave

        // Start wave spawning in the enemy spawner
        if (enemySpawner != null)
        {
            enemySpawner.GetComponent<SpawnerController>().CompleteTutorial();
        }
    }
}
