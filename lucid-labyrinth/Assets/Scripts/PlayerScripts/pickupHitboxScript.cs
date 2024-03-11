using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupHitboxScript : MonoBehaviour
{
    GameObject throwableInRange = null;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("thowableObj"))
        {
            throwableInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        throwableInRange = null;
    }

    public GameObject grabableObj()
    {
        return throwableInRange;
    }
}
