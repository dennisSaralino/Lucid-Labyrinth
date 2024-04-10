using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupObjScript : MonoBehaviour
{
    private bool isHeld = false;
    private bool isAirborne = false;
    public GameObject playerHoldPos;
    public GameObject objGlow;
    //public GameObject pickupTrigger;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isHeld) { transform.position = playerHoldPos.transform.position; }
        if (isAirborne)
        {
            //check for when the object hits the ground
            //call the alert() function in basicAI
        }
    }

    public void Hold()
    {
        isHeld = true;
        objGlow.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Throw(Vector3 var)
    {
        
    }

    public void Drop()
    {
        isHeld = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void ToggleGlow(bool glowState)
    {
        if (!isHeld) { objGlow.SetActive(glowState); }
    }
}
