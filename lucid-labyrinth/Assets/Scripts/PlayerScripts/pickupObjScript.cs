using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupObjScript : MonoBehaviour
{
    private bool isHeld = false;
    private bool isThrown = false;
    private Vector3 throwAngle = new Vector3(0, 0, 0);
    public GameObject playerHoldPos;
    public GameObject objGlow;
    //public GameObject pickupTrigger;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isHeld) { transform.position = playerHoldPos.transform.position; }
        else if (isThrown) 
        {
            Debug.Log(throwAngle);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(transform.InverseTransformDirection(throwAngle));
            isThrown = false; 
        }

    }

    public void Hold()
    {
        isHeld = true;
        isThrown = false;
        objGlow.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Throw(Vector3 var)
    {
        isHeld = false;
        isThrown = true;
        throwAngle = var;
        Debug.Log(throwAngle);
    }

    public void Drop()
    {
        isHeld = false;
        isThrown = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void ToggleGlow(bool glowState)
    {
        if (!isHeld) { objGlow.SetActive(glowState); }
    }
}
