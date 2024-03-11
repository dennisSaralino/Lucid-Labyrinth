using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupObjScript : MonoBehaviour
{
    private bool isHeld = false;
    private bool isThrown = false;
    private Vector3 throwAngle = new Vector3(0, 0, 0);
    public GameObject playerHoldPos;

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            transform.position = playerHoldPos.transform.position;
        }
        else if (isThrown)
        {
            GetComponent<Rigidbody>().AddForce(throwAngle);
            isThrown = false;
        }

    }

    public void Hold()
    {
        isHeld = true;
        isThrown = false;
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
    }
}
