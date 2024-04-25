using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupObjScript : MonoBehaviour
{
    private bool isHeld = false;
    private bool isThrown = false;
    private bool isAirborne = false;
    private Vector3 throwAngle = Vector3.zero;
    public GameObject playerHoldPos;
    //public GameObject objGlow;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isHeld) { transform.position = playerHoldPos.transform.position; }
        else if (isThrown)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            isThrown = false;
            isAirborne = true;
        }
        if (isAirborne)
        {
            //check for when the object hits the ground
            //call the alert() function in basicAI
        }
    }

    public void Hold()
    {
        isHeld = true;
        isThrown = false;
        //objGlow.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Throw(Vector3 var)
    {
        isHeld = false;
        isThrown = true;
        throwAngle = var;
    }

    public void Drop()
    {
        isHeld = false;
        isThrown = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    //public void ToggleGlow(bool glowState)
    //{
    //    if (!isHeld) { objGlow.SetActive(glowState); }
    //}
}
