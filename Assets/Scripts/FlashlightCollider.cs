using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightCollider : MonoBehaviour
{
    public Light flashlight;
    public bool IsHitByFlashlight(GameObject other)
    {
        Ray forward_ray = new Ray(flashlight.transform.position, flashlight.transform.forward);
        // Increment by 10 degrees clockwise
        Ray forward_10_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, 10, 0) * flashlight.transform.forward);
        Ray forward_20_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, 20, 0) * flashlight.transform.forward);
        Ray forward_30_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, 30, 0) * flashlight.transform.forward);
        Ray forward_40_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, 40, 0) * flashlight.transform.forward);
        // Increment by 10 degrees counter-clockwise
        Ray forward_neg_10_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, -10, 0) * flashlight.transform.forward);
        Ray forward_neg_20_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, -20, 0) * flashlight.transform.forward);
        Ray forward_neg_30_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, -30, 0) * flashlight.transform.forward);
        Ray forward_neg_40_ray = new Ray(flashlight.transform.position, Quaternion.Euler(0, -40, 0) * flashlight.transform.forward);
        RaycastHit[] forward_hits;
        RaycastHit[] forward_10_hits;
        RaycastHit[] forward_20_hits;
        RaycastHit[] forward_30_hits;
        RaycastHit[] forward_40_hits;
        RaycastHit[] forward_neg_10_hits;
        RaycastHit[] forward_neg_20_hits;
        RaycastHit[] forward_neg_30_hits;
        RaycastHit[] forward_neg_40_hits;

        if (flashlight.enabled)
        {
            Debug.DrawRay(forward_ray.origin, forward_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_10_ray.origin, forward_10_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_20_ray.origin, forward_20_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_30_ray.origin, forward_30_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_40_ray.origin, forward_40_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_neg_10_ray.origin, forward_neg_10_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_neg_20_ray.origin, forward_neg_20_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_neg_30_ray.origin, forward_neg_30_ray.direction * flashlight.range, Color.red);
            Debug.DrawRay(forward_neg_40_ray.origin, forward_neg_40_ray.direction * flashlight.range, Color.red);
            forward_hits = Physics.RaycastAll(forward_ray, flashlight.range);
            foreach (RaycastHit hit in forward_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_10_hits = Physics.RaycastAll(forward_10_ray, flashlight.range);
            foreach (RaycastHit hit in forward_10_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_20_hits = Physics.RaycastAll(forward_20_ray, flashlight.range);
            foreach (RaycastHit hit in forward_20_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_30_hits = Physics.RaycastAll(forward_30_ray, flashlight.range);
            foreach (RaycastHit hit in forward_30_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_40_hits = Physics.RaycastAll(forward_40_ray, flashlight.range);
            foreach (RaycastHit hit in forward_40_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_neg_10_hits = Physics.RaycastAll(forward_neg_10_ray, flashlight.range);
            foreach (RaycastHit hit in forward_neg_10_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_neg_20_hits = Physics.RaycastAll(forward_neg_20_ray, flashlight.range);
            foreach (RaycastHit hit in forward_neg_20_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_neg_30_hits = Physics.RaycastAll(forward_neg_30_ray, flashlight.range);
            foreach (RaycastHit hit in forward_neg_30_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }
            forward_neg_40_hits = Physics.RaycastAll(forward_neg_40_ray, flashlight.range);
            foreach (RaycastHit hit in forward_neg_40_hits)
            {
                if (hit.collider.gameObject == other)
                {
                    return true;
                }
            }

        }
        return false;
    }
}
