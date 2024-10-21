using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPrefab = true;
    public int reward = 50;
    public int health = 1;
    public int moveSpeed = 5;
    public GameObject Base1;
    public GameObject Base2;
    public GameObject Player;
    public GameObject ResetButton;
    public Light flashlight;
    public FlashlightCollider flashlightCollider;

    // public GameDataRecorder dataRecorder;   // Data recorder
    public FirebaseDataSender firebaseDataSender;  // Reference to FirebaseDataSender

    void Start()
    {
        // Set the color of the enemy based on the health
        if (health == 3)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (health == 2)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (health == 1)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        if (isPrefab)
        {
            return;
        }
        // Move the enemy towards the closest base
        GameObject Base = Base1;
        if (Vector3.Distance(transform.position, Base1.transform.position) > Vector3.Distance(transform.position, Base2.transform.position))
        {
            Base = Base2;
        }
        int moveDistance = moveSpeed;
        if (flashlightCollider.IsHitByFlashlight(gameObject))
        {
            moveDistance = moveSpeed / 2;
        }
        transform.position = Vector3.MoveTowards(transform.position, Base.transform.position, moveDistance * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the enemy hits the base, destroy the enemy
        if (collision.gameObject == Base1 || collision.gameObject == Base2 || collision.gameObject == Player)
        {
            // stop the game
            Time.timeScale = 0;
            // make the game over text text alpha 1
            GameObject.Find("GameOver").GetComponent<UnityEngine.UI.Text>().color = new Color(1, 0, 0, 1);
            // make the reset button visible
            ResetButton.SetActive(true);
              // Record game data
            int currentWave = FindObjectOfType<SpawnerController>().currentWave;
            FirebaseDataSender.Instance.SendGameResult(false, currentWave, Time.timeSinceLevelLoad);
            

            // Prevent multiple triggers
            this.enabled = false;
        }
    }
}
