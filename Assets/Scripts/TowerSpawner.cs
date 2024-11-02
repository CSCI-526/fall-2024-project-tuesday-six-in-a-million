using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSpawner : MonoBehaviour
{
    public GoldUpdater goldUpdater;  // Reference to the UI updater script
    public GameObject regularTowerPrefab;  // Regular tower prefab
    public GameObject LightTowerPrefab;    // Light tower prefab
    public Text alertText;  // Text to show alerts
    public int shiftGeneration = 0;  // Number of times left shift is pressed
    private int lastGeneration = -1;  // Last generation number
    private bool isShiftPressed = false;  // Whether shift is pressed

    public delegate void TowerSpawnedHandler(TowerController tower);
    public static event TowerSpawnedHandler OnTowerSpawned;

    public void SpawnRegularTower()
    {
        Debug.Log("Spawning regular tower");
        int goldCost = regularTowerPrefab.GetComponent<TowerController>().goldCost;
        if (!goldUpdater.SubtractGold(goldCost))
        {
            alertText.text = "Not enough gold to spawn Regular Tower";
            return;
        }
        GameObject towerObject = Instantiate(regularTowerPrefab, transform.position + new Vector3(2, 0, 0), transform.rotation);
        TowerController tower = towerObject.GetComponent<TowerController>();

        OnTowerSpawned?.Invoke(tower);
        Debug.Log("Regular tower spawned");
    }

    public void SpawnLightTower()
    {
        Debug.Log("Spawning light tower");
        int goldCost = LightTowerPrefab.GetComponent<LightTowerController>().goldCost;
        if (!goldUpdater.SubtractGold(goldCost))
        {
            alertText.text = "Not enough gold to spawn Light Tower";
            return;
        }
        GameObject tower = Instantiate(LightTowerPrefab, transform.position + new Vector3(2, 0, 0), transform.rotation);
        tower.SetActive(true);
        Debug.Log("Light tower spawned");
    }

    public void SpawnTower() {
        // press q to spawn regular tower, press e to spawn light tower
        if (shiftGeneration != lastGeneration)
        {
            lastGeneration = shiftGeneration;
            int regularTowerCost = regularTowerPrefab.GetComponent<TowerController>().goldCost;
            int lightTowerCost = LightTowerPrefab.GetComponent<LightTowerController>().goldCost;
            alertText.text = "Press Q to spawn Regular Tower (" + regularTowerCost + "), Press E to spawn Light Tower (" + lightTowerCost + ")";
            alertText.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnRegularTower();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnLightTower();
        }
    }

    public void DetectSpawnTower() {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isShiftPressed = true;
            SpawnTower();
        } else {
            if (isShiftPressed)
            {
                shiftGeneration++;
            }
            isShiftPressed = false;
            alertText.gameObject.SetActive(false);
        }
    }
}
