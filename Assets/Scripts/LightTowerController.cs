using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTowerController : MonoBehaviour
{
    // Start is called before the first frame update
    public int goldCost = 100;
    public int range = 10;
    public float chargeSpeed = 0.2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectLightCollision();
    }

    void DetectLightCollision()
    {
        int rayCount = 36;
        float angleIncrement = 360f / rayCount;
        List<RaycastHit> allHits = new List<RaycastHit>();

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleIncrement;
            Ray ray = new Ray(transform.position, Quaternion.Euler(0, angle, 0) * transform.forward);

            // Draw the ray
            Debug.DrawRay(ray.origin, ray.direction * range, Color.red);

            // Perform the raycast and add hits to the list
            RaycastHit[] hits = Physics.RaycastAll(ray, range);
            allHits.AddRange(hits);
        }

        // deduplicate hits
        HashSet<GameObject> uniqueHitObjects = new HashSet<GameObject>();

        // Check if any regular tower is hit by the light
        foreach (RaycastHit hit in allHits)
        {
            GameObject hitObject = hit.collider.gameObject;
            if (uniqueHitObjects.Contains(hitObject))
            {
                continue;
            }
            uniqueHitObjects.Add(hitObject);
            // if tower layer is hit
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tower"))
            {
                TowerController tower = hit.collider.gameObject.GetComponent<TowerController>();
                if (tower != null)
                {
                    tower.ChargeTowerCustomSpeed(chargeSpeed);
                }
            }
        }
    }
}
