using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupHitboxScript : MonoBehaviour
{
    GameObject throwableInRange = null;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ThrowableObj")) { 
            throwableInRange = other.gameObject.transform.parent.gameObject;
            other.gameObject.transform.parent.gameObject.GetComponent<pickupObjScript>().ToggleGlow(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        throwableInRange = null;
        if (other.CompareTag("ThrowableObj")) { other.gameObject.transform.parent.gameObject.GetComponent<pickupObjScript>().ToggleGlow(false); }
    }

    public GameObject grabableObj()
    {
        return throwableInRange;
    }
}
