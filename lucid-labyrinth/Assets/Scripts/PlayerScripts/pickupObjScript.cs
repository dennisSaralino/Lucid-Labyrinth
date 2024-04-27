
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupObjScript : MonoBehaviour
{
    private bool isHeld = false;
    private bool isAirborne = false;
    private bool hitGround = false;
    private Vector3 throwAngle = Vector3.zero;
    private GameObject playerHoldPos;
    private BoxCollider objCollider;
    private GameObject[] monsters;
    public string throwableType;

    private void Start()
    {
        playerHoldPos = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().holdPos;
        objCollider = GetComponentInChildren<BoxCollider>();
        //monsters = GameObject.FindGameObjectWithTag("Monster").GetComponentsInChildren<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isHeld) { transform.position = playerHoldPos.transform.position; }
        if (isAirborne)
        {
            if (hitGround)
            {
                foreach (GameObject x in monsters)
                {
                    x.GetComponent<basicAI>().alert(transform);
                }
            }
        }
    }

    public void Hold()
    {
        isHeld = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Throw(Vector3 thVec)
    {
        isHeld = false;
        GetComponent<Rigidbody>().isKinematic = false;
        isAirborne = true;
        GetComponent<Rigidbody>().AddForce(thVec);
    }

    public void Drop()
    {
        isHeld = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public bool isKey()
    {
        return gameObject.CompareTag("Key");
    }
}
